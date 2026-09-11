#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Minimal MQTT 3.1.1 publisher —— stdlib only（供 scripts/verify-emqx-e2e.sh 路径②使用）。

为什么不用 EMQX REST /api/v5/publish：
  EMQX 5.x 的 REST 写接口需要 **API Key**（Dashboard 的 basic auth 对这些端点返回 401，
  见 /api/v5/nodes 无论带不带 auth 都 401，而 /api/v5/status 是公开端点返回 200）。
  直接走 MQTT 协议发布既绕开了 REST 鉴权，又更贴近路径②的真实链路（Edge → Broker → 桥）。

用法：
  python3 scripts/mqtt_publish.py <host> <port> <topic> <payload> [client_id] [username] [password]
退出码：0=PUBACK 成功；2=连接失败；3=发布未收到 PUBACK
"""
import socket
import struct
import sys
import time


def encode_remaining_length(n: int) -> bytes:
    """MQTT 变长剩余长度编码（每字节 7 位 +  continuation bit）。"""
    out = b""
    while True:
        b = n % 128
        n //= 128
        if n > 0:
            b |= 0x80
        out += bytes([b])
        if n <= 0:
            break
    return out


def str_field(s: str) -> bytes:
    b = s.encode("utf-8")
    return struct.pack("!H", len(b)) + b


def connect_pkt(client_id: str, username: str = "", password: str = "") -> bytes:
    flags = 0x02  # Clean Session
    payload = str_field(client_id)
    if username:
        flags |= 0x80  # User Name Flag
        payload += str_field(username)
    if password:
        flags |= 0x40  # Password Flag
        payload += str_field(password)
    body = str_field("MQTT") + bytes([0x04, flags]) + struct.pack("!H", 60) + payload
    return bytes([0x10]) + encode_remaining_length(len(body)) + body


def publish_pkt(topic: str, payload: str, qos: int = 1, pid: int = 1) -> bytes:
    body = str_field(topic)
    if qos > 0:
        body += struct.pack("!H", pid)
    body += payload.encode("utf-8")
    fixed = 0x30 | (qos << 1)  # PUBLISH <<4 | QoS
    return bytes([fixed]) + encode_remaining_length(len(body)) + body


def main() -> int:
    if len(sys.argv) < 5:
        print("usage: mqtt_publish.py <host> <port> <topic> <payload> [client_id] [username] [password]")
        return 1
    host = sys.argv[1]
    port = int(sys.argv[2])
    topic = sys.argv[3]
    payload = sys.argv[4]
    cid = sys.argv[5] if len(sys.argv) > 5 else "plc-e2e-pub-" + str(int(time.time()))
    username = sys.argv[6] if len(sys.argv) > 6 else ""
    password = sys.argv[7] if len(sys.argv) > 7 else ""

    try:
        s = socket.create_connection((host, port), timeout=10)
    except Exception as e:
        print(f"MQTT-CONNECT-FAIL tcp {host}:{port} {type(e).__name__}: {e}")
        return 2
    s.settimeout(10)
    try:
        s.sendall(connect_pkt(cid, username, password))
        hdr = s.recv(2)
        if len(hdr) < 2 or hdr[0] != 0x20:
            print(f"MQTT-CONNECT-FAIL bad CONNACK header: {hdr.hex() if hdr else 'none'}")
            return 2
        body = s.recv(2)
        if len(body) >= 2 and body[1] != 0:
            # 4=用户名密码错误 5=未授权（例如 broker 关闭匿名）
            print(f"MQTT-CONNECT-FAIL CONNACK return code {body[1]}")
            return 2
        s.sendall(publish_pkt(topic, payload))
        ack = s.recv(4)
        if ack and ack[0] == 0x40:
            print(f"MQTT-PUBLISH-OK topic={topic} bytes={len(payload)}")
            return 0
        print(f"MQTT-PUBLISH-FAIL no PUBACK: {ack.hex() if ack else 'none'}")
        return 3
    except socket.timeout:
        print("MQTT-TIMEOUT waiting for CONNACK/PUBACK")
        return 3
    except Exception as e:
        print(f"MQTT-ERROR {type(e).__name__}: {e}")
        return 3
    finally:
        try:
            s.close()
        except Exception:
            pass


if __name__ == "__main__":
    sys.exit(main())
