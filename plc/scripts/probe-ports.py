import socket

cands = [8080, 7070, 3000, 7001, 8888, 9000, 4000, 6000, 7077, 8088,
         7200, 7777, 8800, 9090, 7075, 8081, 5100, 5200, 5300, 5400,
         7100, 7300, 7500, 7600, 7800, 7900, 8000, 8020, 8050, 8200]
ok = []
for p in cands:
    s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    try:
        s.bind(("127.0.0.1", p))
        s.listen(1)
        ok.append(p)
    except OSError:
        pass
    finally:
        s.close()
print("BINDABLE:", ok)
