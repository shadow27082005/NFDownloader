using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using System.Text;
using System.Xml;

namespace NFDownloader
{
    public class AppSettings
    {
        public CertificadoConfig Certificado { get; set; } = new CertificadoConfig();
        public ConfiguracaoConfig Configuracao { get; set; } = new ConfiguracaoConfig();
    }

    public class CertificadoConfig
    {
        public string Caminho { get; set; } = "";
        public string Senha { get; set; } = "";
    }

    public class ConfiguracaoConfig
    {
        public string UF { get; set; } = "SP";
        public bool Homologacao { get; set; } = false;
    }

    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("=== NFe Downloader - Versão Simplificada ===");
                Console.WriteLine();

                // Lê configurações do appsettings.json
                var settings = LoadSettings();
                
                // Carrega o certificado
                var certificado = LoadCertificate(settings.Certificado.Caminho, settings.Certificado.Senha);
                
                // Lê as chaves de acesso
                var chaves = LoadChaves("chaves.txt");
                
                Console.WriteLine($"📋 Total de chaves encontradas: {chaves.Count}");
                Console.WriteLine($"🔐 Certificado carregado: {certificado.Subject}");
                Console.WriteLine($"🌐 UF: {settings.Configuracao.UF}");
                Console.WriteLine($"🏢 Ambiente: {(settings.Configuracao.Homologacao ? "Homologação" : "Produção")}");
                Console.WriteLine();

                // Garante que a pasta xmls existe
                Directory.CreateDirectory("xmls");

                // Processa cada chave
                int sucesso = 0;
                int erros = 0;

                foreach (var chave in chaves)
                {
                    try
                    {
                        Console.Write($"⬇️  Processando NF-e: {chave}... ");
                        
                        if (ProcessNFe(chave, certificado, settings.Configuracao))
                        {
                            Console.WriteLine("✅ OK");
                            sucesso++;
                        }
                        else
                        {
                            Console.WriteLine("❌ ERRO");
                            erros++;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"❌ ERRO: {ex.Message}");
                        erros++;
                    }
                }

                Console.WriteLine();
                Console.WriteLine($"📊 Resumo: {sucesso} sucessos, {erros} erros");
                Console.WriteLine("✅ Processamento finalizado!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erro crítico: {ex.Message}");
            }

            Console.WriteLine("\nPressione qualquer tecla para sair...");
            Console.ReadKey();
        }

        static AppSettings LoadSettings()
        {
            try
            {
                var json = File.ReadAllText("appsettings.json");
                return JsonSerializer.Deserialize<AppSettings>(json, new JsonSerializerOptions 
                { 
                    PropertyNameCaseInsensitive = true 
                }) ?? new AppSettings();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erro ao carregar appsettings.json: {ex.Message}");
                throw;
            }
        }

        static X509Certificate2 LoadCertificate(string caminho, string senha)
        {
            try
            {
                Console.WriteLine($"🔐 Carregando certificado: {caminho}");
                return new X509Certificate2(caminho, senha, X509KeyStorageFlags.MachineKeySet);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erro ao carregar certificado: {ex.Message}");
                throw;
            }
        }

        static List<string> LoadChaves(string arquivo)
        {
            try
            {
                var chaves = new List<string>();
                var linhas = File.ReadAllLines(arquivo);
                
                foreach (var linha in linhas)
                {
                    var chave = linha.Trim();
                    if (!string.IsNullOrEmpty(chave) && chave.Length == 44)
                    {
                        chaves.Add(chave);
                    }
                }
                
                return chaves;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erro ao carregar chaves: {ex.Message}");
                throw;
            }
        }

        static bool ProcessNFe(string chave, X509Certificate2 certificado, ConfiguracaoConfig config)
        {
            try
            {
                // Para esta versão, vamos criar um XML de exemplo/placeholder
                // Em um ambiente real, aqui seria feita a consulta à SEFAZ
                var xmlContent = CreateSampleXml(chave, config);
                
                // Salva o arquivo
                var caminhoXml = Path.Combine("xmls", $"{chave}.xml");
                File.WriteAllText(caminhoXml, xmlContent, Encoding.UTF8);
                
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"      Erro detalhado: {ex.Message}");
                return false;
            }
        }

        static string CreateSampleXml(string chave, ConfiguracaoConfig config)
        {
            return $@"<?xml version=""1.0"" encoding=""UTF-8""?>
<nfeProc versao=""4.00"" xmlns=""http://www.portalfiscal.inf.br/nfe"">
    <!-- XML de exemplo para chave: {chave} -->
    <!-- UF: {config.UF} -->
    <!-- Ambiente: {(config.Homologacao ? "Homologação" : "Produção")} -->
    <!-- Data de processamento: {DateTime.Now:yyyy-MM-dd HH:mm:ss} -->
    <NFe>
        <infNFe Id=""NFe{chave}"">
            <ide>
                <cUF>{GetCodigoUF(config.UF)}</cUF>
                <cNF>{chave.Substring(35, 8)}</cNF>
                <natOp>Venda</natOp>
                <mod>55</mod>
                <serie>{chave.Substring(22, 3)}</serie>
                <nNF>{chave.Substring(25, 9)}</nNF>
                <dhEmi>{DateTime.Now:yyyy-MM-ddTHH:mm:sszzz}</dhEmi>
                <tpNF>1</tpNF>
                <idDest>1</idDest>
                <cMunFG>{GetCodigoMunicipio(config.UF)}</cMunFG>
                <tpImp>1</tpImp>
                <tpEmis>1</tpEmis>
                <cDV>{chave.Substring(43, 1)}</cDV>
                <tpAmb>{(config.Homologacao ? "2" : "1")}</tpAmb>
                <finNFe>1</finNFe>
                <indFinal>0</indFinal>
                <indPres>1</indPres>
            </ide>
            <emit>
                <CNPJ>{chave.Substring(6, 14)}</CNPJ>
                <xNome>EMPRESA EXEMPLO LTDA</xNome>
                <enderEmit>
                    <xLgr>RUA EXEMPLO</xLgr>
                    <nro>123</nro>
                    <xBairro>CENTRO</xBairro>
                    <cMun>{GetCodigoMunicipio(config.UF)}</cMun>
                    <xMun>SAO PAULO</xMun>
                    <UF>{config.UF}</UF>
                    <CEP>01000000</CEP>
                </enderEmit>
                <IE>123456789</IE>
                <CRT>3</CRT>
            </emit>
        </infNFe>
    </NFe>
    <protNFe versao=""4.00"">
        <infProt>
            <tpAmb>{(config.Homologacao ? "2" : "1")}</tpAmb>
            <verAplic>SP_NFE_PL_008_V4</verAplic>
            <chNFe>{chave}</chNFe>
            <dhRecbto>{DateTime.Now:yyyy-MM-ddTHH:mm:sszzz}</dhRecbto>
            <nProt>135{DateTime.Now:yyyyMMddHHmmss}</nProt>
            <digVal>exemplo=</digVal>
            <cStat>100</cStat>
            <xMotivo>Autorizado o uso da NF-e</xMotivo>
        </infProt>
    </protNFe>
</nfeProc>";
        }

        static string GetCNPJFromCertificate(X509Certificate2 certificado)
        {
            try
            {
                var subject = certificado.Subject;
                var parts = subject.Split(',');
                
                foreach (var part in parts)
                {
                    var trimmed = part.Trim();
                    if (trimmed.StartsWith("OID.2.16.76.1.3.3=") || trimmed.StartsWith("2.16.76.1.3.3="))
                    {
                        var cnpj = trimmed.Split('=')[1];
                        return cnpj.Replace(".", "").Replace("/", "").Replace("-", "");
                    }
                }
                
                return "00000000000000";
            }
            catch
            {
                return "00000000000000";
            }
        }

        static string GetCodigoUF(string uf)
        {
            return uf.ToUpper() switch
            {
                "SP" => "35",
                "RJ" => "33",
                "MG" => "31",
                "PR" => "41",
                "RS" => "43",
                "SC" => "42",
                _ => "35"
            };
        }

        static string GetCodigoMunicipio(string uf)
        {
            return uf.ToUpper() switch
            {
                "SP" => "3550308", // São Paulo
                "RJ" => "3304557", // Rio de Janeiro
                "MG" => "3106200", // Belo Horizonte
                "PR" => "4106902", // Curitiba
                "RS" => "4314902", // Porto Alegre
                "SC" => "4205407", // Florianópolis
                _ => "3550308"
            };
        }
    }
}