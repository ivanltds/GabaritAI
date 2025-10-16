#!/bin/bash
set -e

###############################################
# 🔧 CONFIGURAÇÕES DO DEPLOY
###############################################
FTP_HOST="ftp://GabaritAI.somee.com"
FTP_USER="Ivanltds"
FTP_PASS="Senha_AQUI!"          # ⚠️ substitua
FTP_PATH="/www.GabaritAI.somee.com"
OPENAI_KEY="Key_AQUI"      # ⚠️ substitua pela sua OpenAI Key

###############################################
echo "=================================="
echo "🚀 DEPLOY AUTOMÁTICO DO GABARITAI"
echo "=================================="

# Instalar dependências
echo "🌐 Instalando dependências FTP..."
sudo apt-get update -y >/dev/null
sudo apt-get install -y lftp >/dev/null

# Publicar o app
echo "📦 Publicando aplicação..."
dotnet publish -c Release -o ./publish

# Remover duplicatas de web.config
echo "🧹 Limpando duplicatas de web.config..."
find ./publish -type f -name "web.config" ! -path "./publish/web.config" -delete || true

# Injetar a API_KEY no web.config
echo "🔑 Inserindo API_KEY no web.config..."
WEB="./publish/web.config"

if grep -q "<environmentVariables>" "$WEB"; then
  # já existe o bloco, só adiciona a linha
  sed -i "/<environmentVariables>/a \      <environmentVariable name=\"API_KEY\" value=\"$OPENAI_KEY\" />" "$WEB"
else
  # adiciona o bloco completo antes de </aspNetCore>
  sed -i "/<\/aspNetCore>/i \    <environmentVariables>\n      <environmentVariable name=\"API_KEY\" value=\"$OPENAI_KEY\" />\n    </environmentVariables>" "$WEB"
fi

# Verificação
echo "✅ web.config atualizado:"
grep -A3 "environmentVariable" "$WEB" || echo "(sem alterações detectadas)"

# Fazer upload via FTP
echo "📡 Enviando arquivos para o Somee..."
lftp -u "$FTP_USER","$FTP_PASS" "$FTP_HOST" <<EOF
set ftp:ssl-allow true
set ftp:use-site-chmod false
set ftp:use-mdtm off
set ftp:use-allo off
set xfer:clobber on
mirror -R ./publish "$FTP_PATH" --parallel=5 --verbose --ignore-time --no-perms --overwrite
bye
EOF

# Resultado
if [ $? -eq 0 ]; then
  echo "✅ Deploy concluído com sucesso!"
  echo "🌍 Acesse: http://GabaritAI.somee.com"
else
  echo "❌ Falha no envio via FTP."
  exit 1
fi
