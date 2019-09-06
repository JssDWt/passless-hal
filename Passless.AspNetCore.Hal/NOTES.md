In order to collect code coverage:
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura

In order to view the output as a webpage:
(dotnet tool install -g dotnet-reportgenerator-globaltool)
reportgenerator -reports:Tests/**/coverage.cobertura.xml -targetdir:Report -reportTypes:HtmlInline_AzurePipelines;Cobertura