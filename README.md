# Morada — Gestão Imobiliária

[![CI](https://github.com/Pssolochi82/imobiliaria-aspnet-core/actions/workflows/ci.yml/badge.svg)](https://github.com/Pssolochi82/imobiliaria-aspnet-core/actions/workflows/ci.yml)
[![Licença MIT](https://img.shields.io/badge/licen%C3%A7a-MIT-blue.svg)](LICENSE)

Aplicação web para gerir clientes, imóveis, interesses de compra e visitas. O projeto combina um dashboard comercial, pesquisa e filtragem, operações CRUD completas e uma experiência responsiva em português.

> Este projeto nasceu em contexto de aula como projeto final da UFCD de ASP.NET Core. A primeira versão foi construída com ASP.NET Web Forms e .NET Framework 4.7.2. Em 2026, o código foi revisto em profundidade e migrado para ASP.NET Core MVC em .NET 10, com uma arquitetura, interface e estratégia de testes adequadas a portfólio.

## Funcionalidades

- Dashboard com indicadores, imóveis recentes e próximas visitas;
- Gestão de clientes, contactos e moradas;
- Carteira de imóveis com pesquisa, filtro por estado e detalhe comercial;
- Registo de perfis de interesse por zona, tipologia e orçamento;
- Agenda de visitas com estados, notas e relações entre cliente e imóvel;
- Validação no cliente e no servidor, mensagens de sucesso e proteção antiforgery;
- Paginação, navegação responsiva e interface acessível;
- Dados de demonstração fictícios, criados automaticamente no primeiro arranque;
- Endpoint de monitorização em `/health`.

## Tecnologias

- .NET 10 e ASP.NET Core MVC;
- Entity Framework Core 10.0.11;
- SQLite e migrações versionadas;
- Razor Views, Bootstrap e CSS personalizado;
- xUnit e EF Core InMemory para testes;
- GitHub Actions para build e testes em cada push ou pull request.

## Arquitetura

O projeto adota uma estrutura MVC pragmática para manter o domínio simples sem esconder decisões importantes:

```text
src/Imobiliaria.Web/
├── Controllers/       # Fluxos HTTP, validação de referências e prevenção de overposting
├── Data/              # DbContext, migrações e dados iniciais
├── Models/            # Entidades, relações e enumerações do domínio
├── Services/          # Consultas agregadas do dashboard
├── ViewModels/        # Contratos específicos para formulários e listagens
├── Views/             # Interface Razor reutilizável e responsiva
└── wwwroot/           # CSS, JavaScript e dependências estáticas locais

tests/Imobiliaria.Tests/
└── Testes de regras, validação, normalização e consultas agregadas
```

As consultas são assíncronas e usam `AsNoTracking` quando os dados são apenas de leitura. Os formulários trabalham com ViewModels e aplicam campos explicitamente às entidades, evitando mass assignment. As relações destrutivas são restringidas quando existem visitas associadas.

## Executar localmente

Pré-requisito: [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0).

```bash
git clone https://github.com/Pssolochi82/imobiliaria-aspnet-core.git
cd imobiliaria-aspnet-core
dotnet restore
dotnet run --project src/Imobiliaria.Web
```

O endereço local é apresentado no terminal. No primeiro arranque, as migrações criam `imobiliaria.db` e o sistema adiciona dados demonstrativos. O ficheiro da base local é ignorado pelo Git.

## Testes e qualidade

```bash
dotnet build --configuration Release
dotnet test --configuration Release --collect:"XPlat Code Coverage"
```

O build trata avisos como erros e aplica os analisadores recomendados do SDK. A integração contínua repete restore, build e testes num ambiente limpo.

## Evolução do projeto

A revisão da versão original identificou problemas funcionais e estruturais típicos de um primeiro projeto académico: caminhos absolutos para a base de dados, SQL montado por concatenação, eventos Web Forms inconsistentes, validação insuficiente, markup inválido, dependências e ficheiros do Visual Studio incluídos no projeto, além da ausência de testes e documentação.

A modernização não altera o objetivo pedagógico original. Em vez disso, preserva as entidades centrais — clientes, casas/imóveis, interesses e visitas — e demonstra como o mesmo problema pode ser resolvido hoje com separação de responsabilidades, persistência portátil, segurança por defeito e experiência de utilização cuidada.

Consulte [docs/CODE_REVIEW.md](docs/CODE_REVIEW.md) para a análise técnica do código original e [docs/ARCHITECTURE.md](docs/ARCHITECTURE.md) para as decisões da versão atual.

## Dados e privacidade

O ficheiro `.mdf` da versão académica não faz parte deste repositório. Todos os nomes, emails, telefones, moradas, imóveis e visitas usados na demonstração atual são fictícios e utilizam o domínio reservado `example.test`.

## Licença

Distribuído sob a licença MIT. Consulte [LICENSE](LICENSE).
