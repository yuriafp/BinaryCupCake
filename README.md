# BinaryCupcake
 O BinaryCupcake é uma aplicação web interativa, construída com Blazor e .NET, oferecendo uma plataforma moderna para pedidos de cupcakes online.

Desenvolvido como parte do Projeto Integrador Transdisciplinar em Ciência da Computação II, o projeto foca na usabilidade, acessibilidade e integração com tecnologias de ponta para proporcionar uma experiência de compra fluida e intuitiva.
Dúvidas ou sugestões?

Entre em contato: yuri13_yuri@hotmail.com

## 📋 Funcionalidades
- Cadastro de usuários: Permite que os usuários se registrem com informações pessoais e seguras.
- Cadastro de produtos
- Integração com serviços externos
- Autenticação segura
- Configuração de permissões
- Interface responsiva

## 🚀 Tecnologias
- [C#](https://learn.microsoft.com/en-us/dotnet/csharp/)
- [Blazor WebAssembly](https://blazor.net/)
- [PostgreSQL](https://www.postgresql.org/)
- [Microsoft Azure](https://azure.microsoft.com/)

## 🛠️ Como executar o projeto
1. Clone o repositório:
   ```bash
   git clone https://github.com/yuriafp/BinaryCupCake.git
   ```
2. Navegue para o diretório do projeto:
   ```bash
   cd BinaryCupCake
   ```
 3. Configure o banco de dados:

- Certifique-se de ter o PostgreSQL rodando.
- Crie o banco de dados conforme especificado no arquivo de configuração.
- Execute os scripts de migração:
   ```bash
   dotnet ef database update
   ```
   
 4. Configure o arquivo de ambiente:
- Crie um arquivo appsettings.json (se necessário).
- Preencha as configurações de conexão com o banco e outras chaves.
   
 5. Configure o banco de dados:
   ```bash
   dotnet run --project BinaryCupCake.Server
   ```

 6. Acesse a aplicação no navegador:
- Localmente: http://localhost:5000.
- Em produção: https://binarycupcake-hah0gwbgetdrdpc3.canadacentral-01.azurewebsites.net.
