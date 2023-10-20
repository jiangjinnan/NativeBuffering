using Microsoft.Extensions.ObjectPool;

namespace NativeBuffering
{
    public static class BufferedObjectWriteContextScopeExtensions
    {
        private static readonly ObjectPool<BufferedObjectWriteContextScope> _pool = new DefaultObjectPoolProvider().Create<BufferedObjectWriteContextScope>();
        public static  BufferedObjectWriteContextScope GetWriteContextScope(this BufferedObjectWriteContext writeContext, int fieldCount)
        { 
            var scope = _pool.Get();
            scope.Initialize(writeContext);
            writeContext.Advance(sizeof(int) * fieldCount);
            return scope;
        }

        public static void ReleaseWriteContextScope(this BufferedObjectWriteContext writeContext, BufferedObjectWriteContextScope writeContextScope)
        {
            writeContextScope.Release();
            _pool.Return(writeContextScope);
        }
    }
}
