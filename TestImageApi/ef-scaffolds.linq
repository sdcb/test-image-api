<Query Kind="Statements">
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>Newtonsoft.Json.Linq</Namespace>
</Query>

string projectFolder = new FileInfo(Util.CurrentQueryPath).Directory!.ToString();
string provider = "Microsoft.EntityFrameworkCore.SqlServer";

Directory.SetCurrentDirectory(projectFolder);

{
	string contextName = "ImageDB";
	string connectionStringName = "DBConnectionString";
	string options = string.Join(" ", new[]
	{
		"--data-annotations",
		"--force",
		$"--context {contextName}",
		"--output-dir DB",
		"--verbose",
	});
	string cmd = $"dotnet ef dbcontext scaffold Name={connectionStringName} {provider} {options}";
	Util.Cmd(cmd.Dump("command text"));
}