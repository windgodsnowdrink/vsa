#:sdk Microsoft.NET.Sdk.Web.BlazorWebAssembly
#:package Microsoft.AspNetCore.Components.WebAssembly@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true

/*
// 初始化加密服务
const cryptoService = new WebCryptoService();
await cryptoService.initializeAsync('your-strong-master-key-here');

// 加密数据
const sensitiveData = '需要加密的敏感数据';
const encrypted = await cryptoService.encryptAsync(sensitiveData);
console.log('加密结果:', encrypted);
// 输出示例: {iv: "a1b2c3d4e5f6...", cipherText: "1a2b3c4d5e6f..."}

// 解密数据
const decrypted = await cryptoService.decryptAsync(encrypted);
console.log('解密结果:', decrypted);
// 输出: "需要加密的敏感数据"

// 通过Interop调用
const result = await window['CryptoInterop'].encrypt('test data');
console.log('Blazor加密结果:', result);

const plain = await window['CryptoInterop'].decrypt(result);
console.log('Blazor解密结果:', plain);

*/
// 1. 加密服务核心实现
export class WebCryptoService {
    private static readonly ALGORITHM = 'AES-GCM';
    private static readonly KEY_USAGES: KeyUsage[] = ['encrypt', 'decrypt'];
    private static readonly KEY_LENGTH = 256;
    
    private _cryptoKey: CryptoKey | null = null;
    private readonly _keyStorageKey = 'webcrypto_symmetric_key';

    // 初始化密钥(使用HKDF派生)
    public async initializeAsync(masterKey: string): Promise<void> {
        const rawKey = new TextEncoder().encode(masterKey);
        const importedKey = await crypto.subtle.importKey(
            'raw',
            rawKey,
            { name: 'HKDF' },
            false,
            ['deriveKey']
        );

        this._cryptoKey = await crypto.subtle.deriveKey(
            {
                name: 'HKDF',
                salt: new Uint8Array(16),
                info: new TextEncoder().encode('AES-GCM Key Derivation'),
                hash: 'SHA-256'
            },
            importedKey,
            { name: ALGORITHM, length: KEY_LENGTH },
            false,
            KEY_USAGES
        );
    }

    // 加密数据
    public async encryptAsync(plainText: string): Promise<EncryptedData> {
        if (!this._cryptoKey) throw new Error('Crypto key not initialized');
        
        const iv = crypto.getRandomValues(new Uint8Array(12));
        const encoded = new TextEncoder().encode(plainText);
        
        const cipherText = await crypto.subtle.encrypt(
            { name: ALGORITHM, iv },
            this._cryptoKey,
            encoded
        );

        return {
            iv: Array.from(iv).map(b => b.toString(16).padStart(2, '0')).join(''),
            cipherText: Array.from(new Uint8Array(cipherText))
                .map(b => b.toString(16).padStart(2, '0')).join('')
        };
    }

    // 解密数据
    public async decryptAsync(encryptedData: EncryptedData): Promise<string> {
        if (!this._cryptoKey) throw new Error('Crypto key not initialized');
        
        const iv = new Uint8Array(
            encryptedData.iv.match(/.{2}/g)!.map(b => parseInt(b, 16))
        );
        
        const cipherText = new Uint8Array(
            encryptedData.cipherText.match(/.{2}/g)!.map(b => parseInt(b, 16))
        );

        const plainText = await crypto.subtle.decrypt(
            { name: ALGORITHM, iv },
            this._cryptoKey,
            cipherText
        );

        return new TextDecoder().decode(plainText);
    }
}

// 2. Blazor集成
export function initializeCryptoService(dotNetHelper: any): void {
    const service = new WebCryptoService();
    
    // 注册.NET可调用方法
    window['CryptoInterop'] = {
        initialize: async (masterKey: string) => {
            await service.initializeAsync(masterKey);
            return true;
        },
        encrypt: async (plainText: string) => {
            return await service.encryptAsync(plainText);
        },
        decrypt: async (encryptedData: EncryptedData) => {
            return await service.decryptAsync(encryptedData);
        }
    };
}

// 3. 类型定义
interface EncryptedData {
    iv: string;
    cipherText: string;
}

// 4. .NET端服务封装
public sealed class CryptoService : IDisposable
{
    private readonly IJSRuntime _jsRuntime;
    private IJSObjectReference _module;
    
    public CryptoService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }
    
    public async ValueTask InitializeAsync(string masterKey)
    {
        _module = await _jsRuntime.InvokeAsync<IJSObjectReference>(
            "import", "./js/cryptoService.js");
        
        await _module.InvokeVoidAsync("initializeCryptoService", masterKey);
    }
    
    public async ValueTask<string> EncryptAsync(string plainText)
    {
        var result = await _jsRuntime.InvokeAsync<EncryptedData>(
            "CryptoInterop.encrypt", plainText);
        
        return System.Text.Json.JsonSerializer.Serialize(result);
    }
    
    public async ValueTask<string> DecryptAsync(string encryptedJson)
    {
        var data = System.Text.Json.JsonSerializer.Deserialize<EncryptedData>(encryptedJson);
        return await _jsRuntime.InvokeAsync<string>(
            "CryptoInterop.decrypt", data);
    }
    
    public void Dispose() => _module?.DisposeAsync();
}