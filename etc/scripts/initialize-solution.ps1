abp install-libs

cd src/Pusula.Student.Automation.DbMigrator && dotnet run && cd -

cd src/Pusula.Student.Automation.Blazor && dotnet dev-certs https -v -ep openiddict.pfx -p ed054810-14b0-4071-a598-51d95cecdfbc




exit 0