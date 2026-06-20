using System;
using System.Threading;
using UnityEngine;

namespace Utils
{
    public class OneTimeTokenSource : IDisposable
    {
        public int Gen { get; private set; }
        public bool IsReleased => _source == null;
        
        private CancellationTokenSource _source;
        private readonly Func<CancellationTokenSource> _factory = () => CancellationTokenSource.CreateLinkedTokenSource(Application.exitCancellationToken);

        public OneTimeTokenSource()
        {
            
        }

        public OneTimeTokenSource(Func<CancellationTokenSource> factory)
        {
            _factory = factory;
        }
        
        public TokenHolder NewToken()
        {
            ReleaseToken();
            Gen++;

            _source = _factory();
            
            return new TokenHolder(this, _source.Token);
        }

        public void ReleaseToken()
        {
            _source?.Cancel();
            _source?.Dispose();
            _source = null;
        }

        public void Dispose()
        {
            ReleaseToken();
        }
    }

    public struct TokenHolder : IDisposable
    {
        public CancellationToken Token;
        
        private readonly WeakReference<OneTimeTokenSource> _tokenSource;
        private readonly int _gen;
        
        public TokenHolder(OneTimeTokenSource tokenSource, CancellationToken token)
        {
            _tokenSource = new WeakReference<OneTimeTokenSource>(tokenSource);
            _gen = tokenSource.Gen;
            Token = token;
        }
        
        public void Dispose()
        {
            if (_tokenSource.TryGetTarget(out var tokenSource) && tokenSource.Gen == _gen)
            {
                tokenSource.ReleaseToken();
            }
        }
    }
}