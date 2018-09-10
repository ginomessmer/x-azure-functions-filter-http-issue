## What is this all about?

The current Azure Functions runtime provides a way to run certain checks before the actual function is being executed. This allows us to use it in specific scenarios like authorization, request filtering, validation and so on.

These checks are known as [Filters](https://github.com/Azure/azure-webjobs-sdk/wiki/Function-Filters), a well known concept adopted from ASP.NET. Filters are being built as attributes. Filters can be triggered by HTTP requests or any other Function triggers, making them universal to use.

Though it’s possible to stop the execution from a filter, there’s no way to respond to a HTTP request and tell what exactly happened other than just providing an Internal Server Error (500) as HTTP response by default. This is the scope of this repository.

## The issue

If you take a look into `FunctionsFilterHttpIssue.Filter.BannedNameAttribute`, you’ll notice it throws an ArgumentException when ever the client passes a banned name (that is part of the BannedNames array). If we were about to translate it into common HTTP terms, that would be a 403 - Forbidden for instance, right?

However, there’s no way for us to pass any status codes back to the runtime. The runtime interprets all exceptions in our Function as 500 - Internal Server Error, though that’s not entirely right in this case in theory. It’s just an invalid request in this aspect.



![1536579551008](assets/1536579551008.png)

_**Expected behavior**: Pass HTTP 400/403 (or any other status code) back to the client as response plus return the exception message to the response content._

_**Actual behavior**: Pass HTTP 500 back to the client as response plus don’t provide a response content (the response content is actually completely empty)._

---

### Possible solutions

Some ideas I have in mind, based on other frameworks and their middlewares I’ve worked with in the past.

#### 1. Provide the HTTP context and inject it to the Filter

```csharp
public class SampleFilterAttribute : FunctionInvocationFilterAttribute
{
    public override async Task OnExecutingAsync(FunctionExecutingContext executingContext, CancellationToken cancellationToken) 
    {
        // ...
        base.Context.Http.Abort(StatusCode.Forbidden, "The name you provided belongs to the list of banned names.");
        // This aborts the whole request and returns a HTTP response with the status code and the error message as body content.
    }
}
```



#### 2. Provide a new exception 

… that automatically turns the exception into a HTTP response when applicable:

```csharp
public class SampleFilterAttribute : FunctionInvocationFilterAttribute
{
    public override async Task OnExecutingAsync(FunctionExecutingContext executingContext, CancellationToken cancellationToken) 
    {
        // ...
        throw new FilterHttpException(StatusCode.Forbidden, "The name you provided belongs to the list of banned names.");
        
        // This returns a proper HTTP response when ever this Filter is being called by a HTTP trigger.
        // If that's not the case -> Log it somewhere with its status code and handle it as common exception.
    }
}
```



---

### More references

There are already several threads on GitHub, MSDN, StackOverflow covering the exact same issue, though they haven’t been addressed yet:

- https://github.com/Azure/azure-webjobs-sdk/issues/1314
- https://social.msdn.microsoft.com/Forums/en-US/ec9b73d7-b380-46af-b98a-1370c2b8242a