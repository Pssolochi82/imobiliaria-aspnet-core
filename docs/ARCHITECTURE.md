# Decisões de arquitetura

## Objetivos

- Preservar os conceitos do exercício original;
- Eliminar dependências da máquina de desenvolvimento;
- Tornar os fluxos demonstráveis com um único comando;
- Mostrar práticas atuais sem criar camadas cerimoniais desnecessárias;
- Manter o projeto legível para quem avalia um portfólio.

## Aplicação MVC única

O sistema usa um único projeto ASP.NET Core MVC e um projeto de testes. Para esta dimensão, separar domínio, aplicação e infraestrutura em assemblies independentes acrescentaria mais referências do que valor. As fronteiras continuam explícitas através de pastas, namespaces, ViewModels e serviços injetados.

Os controllers coordenam pedidos HTTP e persistência. O dashboard, por agregar vários conjuntos de dados, usa um serviço dedicado. Se novas regras de negócio crescerem, os fluxos podem ser extraídos para serviços sem alterar as Views.

## Persistência

SQLite torna a demonstração portátil. O `AppDbContext` define:

- email único por cliente;
- precisão monetária e de área;
- enumerações persistidas como texto legível;
- índices para estado/zona de imóveis e data de visitas;
- remoção em cascata apenas para interesses;
- restrição de remoção quando existem visitas;
- proprietário opcional, convertido em `null` se o cliente for removido.

A migração `InitialCreate` é aplicada no arranque. O seeder só é executado quando ainda não existem clientes, por isso é idempotente.

## Segurança

- O Entity Framework parametriza o acesso à base de dados;
- ViewModels impedem que o browser altere propriedades não expostas;
- todos os POST usam antiforgery;
- referências a clientes e imóveis são validadas no servidor;
- recursos destrutivos com dependências são recusados de forma explícita;
- cabeçalhos CSP, `X-Content-Type-Options`, `X-Frame-Options` e `Referrer-Policy` reduzem a superfície do browser;
- o repositório exclui bases locais, configurações privadas e artefactos de build.

## Interface

A identidade visual “Morada” é implementada apenas com HTML e CSS locais. Não existem fontes, imagens ou scripts carregados de terceiros. A interface usa elementos semânticos, labels, estados de foco, navegação por teclado e breakpoints para desktop, tablet e telemóvel.

## Testes

Os testes cobrem a consulta agregada do dashboard, normalização de dados, regras de validação e paginação. O EF Core InMemory isola os testes do sistema de ficheiros. A validação manual complementa a suite com criação de dados, pesquisa, renderização de todas as rotas, migração numa base vazia, endpoint de saúde e viewport móvel.
