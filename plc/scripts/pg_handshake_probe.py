import socket, struct, sys

def probe(host, port=5432, timeout=8):
    try:
        s = socket.create_connection((host, port), timeout=timeout)
    except Exception as e:
        print(f"{host}:{port} CONNECT-FAIL {type(e).__name__}: {e}")
        return
    s.settimeout(timeout)
    try:
        # PostgreSQL StartupMessage (protocol 3.0), plaintext (no SSL request)
        body = b"user\x00plcsaas\x00database\x00plc_aiot_saas\x00\x00"
        pkt = struct.pack("!II", 8 + len(body), 196608) + body
        s.sendall(pkt)
        data = s.recv(2048)
        if not data:
            print(f"{host}:{port} CONNECTED but NO RESPONSE (intercepted / no PG protocol)")
            return
        tag = chr(data[0]) if data else "?"
        names = {"R": "Authentication request (PG ok)", "E": "ErrorResponse (PG ok)",
                 "S": "ParameterStatus (PG ok)", "N": "NegotiateProtocolVersion"}
        print(f"{host}:{port} replied tag='{tag}' -> {names.get(tag,'other')} bytes={len(data)}")
    except socket.timeout:
        print(f"{host}:{port} CONNECTED then HANDSHAKE TIMEOUT -> not speaking PostgreSQL")
    except Exception as e:
        print(f"{host}:{port} handshake error {type(e).__name__}: {e}")
    finally:
        try: s.close()
        except Exception: pass

for h in ("localhost", "127.0.0.1"):
    probe(h)
