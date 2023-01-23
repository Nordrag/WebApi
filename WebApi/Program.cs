ApiHelper.SetUrl("https://jsonplaceholder.typicode.com/");
//inject with dependency, but for now just create a new error handler
ErrorHandler handler = new ErrorHandler();

//without error handling
var posts = await ApiHelper.GetRequest<List<UserPostModel>>("posts");

Console.WriteLine(posts[5].body);

//with handler
var error = await handler.TryCatchAsync(ApiHelper.GetRequest<List<UserPostModel>>("wrong path"));

Console.WriteLine("\n");

Console.WriteLine(error.Message);

Console.WriteLine("\n");

//with handler and param
int param = 1;
var post = await handler.TryCatchAsync(ApiHelper.GetRequest<List<CommentModel>>($"comments?postId={param}"));

//handle error
if(post.Success) { Console.WriteLine(post.Result[1].email); }