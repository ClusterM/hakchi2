using System;

namespace com.clusterrr.ssh
{
    public class SshClientException : Exception
    {
        public SshClientException(string message) : base(message) { }
    }
}
