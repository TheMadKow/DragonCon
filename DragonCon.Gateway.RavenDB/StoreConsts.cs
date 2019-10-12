using System;
using System.Collections.Generic;
using System.Text;

namespace DragonCon.RavenDB
{
    public static class StoreConsts
    {
        public const string ConnectionString = "https://a.free.dragoncon.ravendb.cloud";
        public const string CertificatePath = "..\\..\\Certificates\\free.dragoncon.client.certificate.pfx";
        
        public const string DatabaseName_Developement = "DEV_DragonCon";
        public const string DatabaseName_Staging = "STG_DragonCon";
        public const string DatabaseName_Production = "DragonCon";

    }
}
