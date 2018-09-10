using Microsoft.Azure.WebJobs.Host;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace FunctionsFilterHttpIssue.Filter
{
    public class BannedNamesAttribute : FunctionInvocationFilterAttribute
    {
        public string[] BannedNames { get; }

        public BannedNamesAttribute(params string[] bannedNames)
        {
            BannedNames = bannedNames;
        }

        public override async Task OnExecutingAsync(FunctionExecutingContext executingContext, CancellationToken cancellationToken)
        {
            var request = executingContext.Arguments.FirstOrDefault(a => a.Key == "req").Value as HttpRequest;

            string name = request.Query["name"];
            var match = BannedNames.FirstOrDefault(n => n.ToLower() == name.ToLower()) != null;

            if (match)
            {
                // Return HTTP 400 Bad Request here

                // Not possible - can't hook to the actual Function context or request
                // throw exception instead (that's actually the "recommended" way as stated in the functions documentation)
                throw new ArgumentException($"{name} is a banned name.");
            }
            else
            {
                await base.OnExecutingAsync(executingContext, cancellationToken);
            }
        }
    }
}
