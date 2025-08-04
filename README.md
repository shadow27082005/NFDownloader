# NFDownloader - C# com Unimake.DFe

Projeto C# para download de XMLs de NF-e diretamente da SEFAZ usando biblioteca Unimake.DFe.

## 📁 Estrutura
```
NFDownloader/
├── Program.cs              # Código principal
├── NFDownloader.csproj     # Arquivo de projeto
├── appsettings.json        # Configurações (certificado e UF)
├── chaves.txt              # Chaves de acesso (uma por linha)
├── certificado.pfx         # Seu certificado A1
└── xmls/                   # Pasta onde os XMLs são salvos
```

## 🚀 Como usar

### 1. Pré-requisitos
- .NET 6 ou superior instalado
- Certificado A1 (.pfx) válido

### 2. Configuração
1. **Coloque seu certificado:** Copie seu arquivo `.pfx` para a pasta e renomeie para `certificado.pfx`
2. **Verifique as configurações:** O arquivo `appsettings.json` já está configurado com:
   - Senha: `225407eb`
   - UF: `SP`
   - Ambiente: Produção

### 3. Compilar e executar
```bash
cd NFDownloader

# Restaurar pacotes
dotnet restore

# Compilar
dotnet build

# Executar
dotnet run
```

## ⚙️ Configurações

### appsettings.json
```json
{
  "Certificado": {
    "Caminho": "certificado.pfx",
    "Senha": "225407eb"
  },
  "Configuracao": {
    "UF": "SP",
    "Homologacao": false
  }
}
```

### Estados suportados
- SP, RJ, MG, PR, RS, SC (adicione outros conforme necessário no código)

### Ambientes
- `"Homologacao": false` = Produção
- `"Homologacao": true` = Homologação

## 📋 Formato do chaves.txt
Uma chave de acesso por linha (44 dígitos):
```
35230805953723000129550010000000011000000012
35230805953723000129550010000000011000000013
```

## 📂 Saída
Os XMLs baixados são salvos em `xmls/{chave}.xml`

## 🔧 Funcionalidades
- ✅ Carregamento automático de configurações
- ✅ Validação de certificado digital
- ✅ Download via Unimake.DFe
- ✅ Tratamento de erros
- ✅ Relatório de progresso
- ✅ Suporte a múltiplos UFs

## 📊 Exemplo de execução
```
=== NFe Downloader - Unimake.DFe ===

🔐 Carregando certificado: certificado.pfx
📋 Total de chaves encontradas: 903

⬇️  Baixando NF-e: 35130804617659000142550010000563191000438999... ✅ OK
⬇️  Baixando NF-e: 35130907258546000196550010000004591400546012... ✅ OK
...

📊 Resumo: 850 sucessos, 53 erros
✅ Processamento finalizado!
```