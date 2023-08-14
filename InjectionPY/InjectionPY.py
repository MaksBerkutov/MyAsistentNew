"""
Работа над библиотекой которая позволит подключаться к серверу через тсп соеденение 
Не зависимо от языка програмирования 
"""

from cryptography.hazmat.primitives.asymmetric import rsa, padding
from cryptography.hazmat.primitives import serialization, hashes
from cryptography.hazmat.backends import default_backend

private_key = rsa.generate_private_key(
    public_exponent=65537,
    key_size=2048,
    backend=default_backend()
)
public_key = private_key.public_key()

message = b"Hello, RSA Encryption!"

# Шифрование
cipher_text = public_key.encrypt(
    message,
    padding.OAEP(
        mgf=padding.MGF1(algorithm=hashes.SHA256()),
        algorithm=hashes.SHA256(),
        label=None
    )
)

# Дешифрование
decrypted_message = private_key.decrypt(
    cipher_text,
    padding.OAEP(
        mgf=padding.MGF1(algorithm=hashes.SHA256()),
        algorithm=hashes.SHA256(),
        label=None
    )
)

print("Original:", message.decode('utf-8'))
print("Encrypted:", cipher_text)
print("Decrypted:", decrypted_message.decode('utf-8'))
