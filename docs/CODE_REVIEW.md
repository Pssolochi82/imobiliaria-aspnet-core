# Revisão técnica da versão académica

## Âmbito

A revisão abrangeu os ficheiros autorais `.aspx`, code-behind C#, master pages, configuração, projeto MSBuild, dependências, CSS e o modelo de dados visível no código. Ficheiros gerados pelo Visual Studio, bibliotecas minificadas e código de terceiros foram inventariados, mas não avaliados linha a linha como autoria do projeto.

O projeto original compila como ASP.NET Web Forms em .NET Framework 4.7.2. Contudo, parte das páginas é compilada apenas no primeiro pedido pelo ASP.NET, pelo que o build isolado não deteta todas as falhas de markup e de ligação de eventos.

## Principais conclusões

| Severidade | Área | Evidência na versão original | Impacto |
| --- | --- | --- | --- |
| Crítica | Persistência | `Inserir.aspx.cs:63-65` monta um `INSERT` com concatenação de texto e contém sintaxe SQL inválida | Risco de injeção SQL e operação de interesse inutilizável |
| Alta | Portabilidade | `IndexC.aspx.cs:10`, `InserirC.aspx.cs:10`, `InserirV.aspx.cs:10` e `Inserir.aspx.cs:21/53` repetem um caminho absoluto na unidade `D:` | A aplicação só funciona na máquina e pasta originais |
| Alta | Fluxo Web Forms | `Inserir.aspx:14` chama `Button1_Click`, mas o code-behind expõe `b_inserir_Click` | Falha em runtime ao submeter o formulário |
| Alta | Markup | `IndexC.aspx` fecha um `asp:Content` que nunca abre e tem um `BoundField` fora do `GridView`; `Inserir.aspx:15` contém `<br /` incompleto | Erros de parsing/renderização em runtime |
| Alta | Dados | `InserirV.aspx.cs:72-74` aceita data e seleções vazias e usa `AddWithValue` com valores textuais para chaves | Registos inválidos, conversões implícitas e erros de base de dados |
| Média | Validação | `InserirC.aspx.cs:50-53` usa `Convert` diretamente sobre texto introduzido | Exceções para anos, quartos e preços inválidos ou formatos culturais diferentes |
| Média | Recursos | Ligações e leitores são geridos de forma inconsistente; `Inserir.aspx.cs` abre a ligação durante `Page_Init` | Trabalho de I/O desnecessário e maior probabilidade de fugas de recursos |
| Média | Manutenção | SQL, ligação e mapeamento estão duplicados em cada página code-behind | Alterações simples exigem editar múltiplos ficheiros e favorecem divergências |
| Média | Navegação | `Site.Master:52-54` aponta para páginas `/Visitas`, `/Cliente` e `/Casa` que não existem | Navegação principal quebrada |
| Média | Conteúdo | `About.aspx` e `Contact.aspx` mantêm o texto de exemplo do template | Aspeto inacabado e pouco adequado a portfólio |
| Média | Frontend | A imagem principal é carregada de um URL externo do Pinterest, sem texto alternativo nem controlo de disponibilidade/licença | Dependência externa, acessibilidade insuficiente e risco de conteúdo indisponível |
| Baixa | Repositório | O ZIP inclui `.vs/`, `packages/`, `.csproj.user`, `.mdf` e `.ldf` | Repositório pesado, ruído e possível exposição de dados locais |

## Problemas por fluxo

### Clientes e interesses

- A página apresentada como “Inserir Cliente” implementa, na realidade, um interesse;
- O controlo `txt_nºass` é declarado no markup mas não participa no SQL;
- A consulta `Select * from T_interesse` é executada e o resultado é imediatamente substituído por outra consulta;
- O primeiro `SqlDataReader` não é fechado antes de reutilizar a ligação;
- O SQL inclui uma coluna sem valor, uma vírgula final e dados sem parâmetros.

### Imóveis

- A inserção usa parâmetros, o que é positivo, mas `AddWithValue` não define tipos e tamanhos;
- Não existe validação de campos obrigatórios, limites, ano, moeda ou proprietário;
- A listagem usa `SELECT *` e expõe diretamente a estrutura física da tabela;
- O código da visita espera uma coluna `morada`, mas o formulário de imóvel não recolhe esse campo.

### Visitas

- O calendário pode não ter uma data selecionada;
- Os dropdowns permitem valores vazios sem validação;
- Handlers de alteração vazios acrescentam ruído;
- A confirmação usa `Response.Write`, escrevendo fora da estrutura normal da master page.

### Estrutura e apresentação

- O padrão Web Forms mistura UI, acesso a dados e regras no mesmo ficheiro;
- `Site.Master` contém muitos scripts de template que não são usados;
- Existem duas estratégias de interface móvel: Bootstrap responsivo e uma master page móvel antiga;
- Não existiam testes, CI, documentação de instalação, `.gitignore` ou histórico Git.

## Alterações aplicadas

| Antes | Depois |
| --- | --- |
| ASP.NET Web Forms / .NET Framework 4.7.2 | ASP.NET Core MVC / .NET 10 |
| ADO.NET e SQL manual em code-behind | Entity Framework Core e consultas LINQ parametrizadas |
| Caminho absoluto para LocalDB `.mdf` | SQLite portátil por configuração |
| Criação implícita e sem versionamento | Migração inicial versionada e dados fictícios idempotentes |
| Entidades ligadas diretamente aos formulários | ViewModels com validação e aplicação explícita de campos |
| Operações síncronas | I/O assíncrono e suporte de `CancellationToken` |
| Listagens sem pesquisa | Pesquisa, filtro, paginação e estados visuais |
| Mensagens por `Response.Write` ou JavaScript | TempData e alertas acessíveis no layout |
| Links e páginas de template | Navegação coerente e conteúdo específico do domínio |
| Imagem remota | Composição visual local em CSS, sem dependências externas |
| Sem testes ou automação | xUnit, cobertura e GitHub Actions |
| Base académica potencialmente sensível | Dados sintéticos `example.test`; `.mdf`, `.ldf` e `.db` ignorados |

## Riscos residuais e próximos passos

A versão de portfólio é uma aplicação demonstrativa e não inclui autenticação ou autorização. Antes de uso real, seria necessário introduzir ASP.NET Core Identity, perfis de acesso, auditoria de alterações, armazenamento de fotografias, backups, proteção de dados persistente e uma base gerida como PostgreSQL ou SQL Server.
