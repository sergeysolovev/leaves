// using System.Threading.Tasks;

// public class GrantAccess : IOperation<VerifyAccessResult>
// {
//     // we need to make parameters injectable
//     // from simple values or other operations

//     public GrantAccess InjectPrincipal(IOperation<Result<ClaimsPrincipal>> dependency)
//     {
//         // Task<Result<ClaimsPrincipal>>
//     }

//     public GrantAccess InjectCode(IOperation<StringResult> dependency)
//     {
//         // Task<StringResult>
//     }

//     public GrantAccess InjectRedirectUrl(IOperation<StringResult> dependency)
//     {
//         // Task<StringResult>
//     }

//     public Task<VerifyAccessResult> ExecuteAsync()
//     {
//         var result = userManager
//             .EnsureUserCreatedAsync(principal).ToAsyncOperation()
//             .Bind(userOp => googleAuthService
//                 .ExchangeAuthCode(code, redirectUrl).ToAsyncOperation()
//                 .Bind(exchangeOp => this
//                     VerifyOAuthExchangeIdentity(
//                         principal: principal,
//                         idToken: exchange.Current.Tokens.IdToken).ToAsyncOperation()
//                     .Bind(verifyOp => userManager
//                         .SaveRefreshTokenAsync(
//                             user: user.Current.User,
//                             token: exchange.Current.Tokens.RefreshToken).ToAsyncOperation()
//                     )
//                 )
//             );
//     }
// }

// public async Task<VerifyAccessResult> GrantAccess(
//     string code,
//     string redirectUrl,
//     ClaimsPrincipal principal)
// {
//     // IOperation<TResult>
//     //  Task<TResult> ExecuteAsync();
//     //  IOperation<TResult> Inject<TDependencyResult>(IOperation<TDependencyResult> dependency);
//     //  IOperation<TResult> Inject<TDependencyResult>(Task<TDependencyResult> dependency);

//     // Unit =

//     var result = userManager
//         .EnsureUserCreatedAsync(principal).ToAsyncOperation()
//         .Bind(userOp => googleAuthService
//             .ExchangeAuthCode(code, redirectUrl).ToAsyncOperation()
//             .Bind(exchangeOp => this
//                 VerifyOAuthExchangeIdentity(
//                     principal: principal,
//                     idToken: exchange.Current.Tokens.IdToken).ToAsyncOperation()
//                 .Bind(verifyOp => userManager
//                     .SaveRefreshTokenAsync(
//                         user: user.Current.User,
//                         token: exchange.Current.Tokens.RefreshToken).ToAsyncOperation()
//                 )
//             )
//         );
// }