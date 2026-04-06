using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Authentication
{
    public interface ISecretProvider
    {
        string GetSecret();
    }
}
