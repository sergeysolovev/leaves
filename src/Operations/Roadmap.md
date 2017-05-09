## Custom awaiter for Operation
```
async IOperation<T> Method(...)
{
    return await ...
}
```
This allows to avoid writing an operation wrapper inside each method.

## Work without Task in Builders
For the most part, builders are not asynchronous, but we allocating lots of Task objects.