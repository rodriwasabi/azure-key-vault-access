// See https://aka.ms/new-console-template for more information
using Microsoft.Azure.KeyVault;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

Console.WriteLine("Retrieving secret...");

var baseUri = Environment.GetEnvironmentVariable("MFLOW_TEST_BASE_URL");
if(string.IsNullOrEmpty(baseUri))
    Console.WriteLine($"Env Var not found: MFLOW_TEST_BASE_URL");

var clientId = Environment.GetEnvironmentVariable("MFLOW_TEST_CLIENT_ID");
if(string.IsNullOrEmpty(clientId))
    Console.WriteLine($"Env Var not found: MFLOW_TEST_CLIENT_ID");

var clientSecret = Environment.GetEnvironmentVariable("MFLOW_TEST_CLIENT_SECRET");
if(string.IsNullOrEmpty(clientSecret))
    Console.WriteLine($"Env Var not found: MFLOW_TEST_CLIENT_SECRET");

var client = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(
    async (string auth, string res, string scope) => 
    {
        var authContext = new AuthenticationContext(auth);
        var credential = new ClientCredential(clientId, clientSecret);
        var result = await authContext.AcquireTokenAsync(res, credential);
        if (result == null)
            throw new InvalidOperationException("Failed to retrieve token");
        return result.AccessToken;
    }
));

try{
var secretData = await client.GetSecretAsync(baseUri, "infrastructure-deployment-values-productionv3");

Console.WriteLine($"Secret: {secretData.Value}");
Console.ReadKey();
}
catch(Exception ex)
{
    Console.WriteLine(ex.Message);
}