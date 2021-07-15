using MSCLoader;
using UnityEngine;
using MSCLoader.PartMagnet;
using System.Linq;

namespace RallyPanel
{
    public class RallyInstrument : Mod
    {
        public override string ID => "Rally_Dash";
        public override string Name => "Rally dash";
        public override string Author => "Arthur";
        public override string Version => "Release 1.0";
        public override string Description => "This mod adds a new LightWeight rally instrument panel! amazing, right?";

        public GameObject Instrument;
        GameObject InstrumentTrigger;
        public BoltMagnet boltmagnet;
        public static bool Bought;
        private static readonly string EncryptionKey = "fart"; //don't ask me why.

        public override void OnNewGame()
        {
            ModSave.Delete("RallyI");
            ModConsole.Log("[Rally Dash] Reseted!");
        }

        public override void OnLoad()
        {
            AssetBundle bundle = AssetBundle.CreateFromMemoryImmediate(Properties.Resources.ri);
            Instrument = Object.Instantiate(bundle.LoadAsset<GameObject>("Dash.prefab"));
            Instrument.AddComponent<ScreenTextHandler>();
            GameObject Stand = Object.Instantiate(bundle.LoadAsset<GameObject>("standprefab.prefab"));
            bundle.Unload(false);
            SaveData RSavedata = ModSave.Load<SaveData>("RallyI", EncryptionKey);

            //Setup the part trigger in code, cause why not? :)
            InstrumentTrigger = new GameObject("Instrument_trigger");
            SphereCollider InstrumentColl = InstrumentTrigger.AddComponent<SphereCollider>();
            InstrumentColl.isTrigger = true;
            InstrumentTrigger.transform.SetParent(GameObject.Find("SATSUMA(557kg, 248)/Dashboard").transform, false);

            Stand.transform.SetParent(GameObject.Find("SATSUMA(557kg, 248)/Dashboard").transform, false);
            Stand.SetActive(false);

            boltmagnet = Instrument.GetComponent<BoltMagnet>();
            boltmagnet.attachmentPoints = new Collider[] { InstrumentColl };
            
            boltmagnet.OnAttach.AddListener(() => Stand.SetActive(true));
            boltmagnet.OnDetach.AddListener(() => Stand.SetActive(false));
            
            InstrumentColl.transform.localPosition = new Vector3(0.31f, 0.5f, 0.9f);
            InstrumentColl.transform.localEulerAngles = new Vector3(270f, 180f, 0f);
            InstrumentColl.radius = 0.1f;
            Bought = RSavedata.PartBought;
            //Save file check.
            if (RSavedata.PartBought || RSavedata.PartInstalled)
            {
                Instrument.transform.position = RSavedata.PartPosition;
                Instrument.transform.localEulerAngles = RSavedata.PartRotation;
                boltmagnet.Setup(RSavedata.PartInstalled, 0, RSavedata.boltsteps);
            }
            else
            {
                Instrument.AddComponent<Shop>();
                Instrument.transform.localPosition = new Vector3(1553.693f, 5.532798f, 740.4529f);
                Instrument.transform.localEulerAngles = new Vector3(359.9369f, 39.20426f, -0.003591374f);
            }
            ModConsole.Log("[Rally Dash] Loaded!");
            
        }
        //SaveData setup.
        public class SaveData 
        {
            public bool PartInstalled = false;
            public bool PartBought;
            public Vector3 PartPosition = Vector3.zero;
            public Vector3 PartRotation = Vector3.zero;
            public int[] boltsteps = {0};
        }

        public override void OnSave() 
        {
            ModSave.Save("RallyI", new SaveData
            {
                PartInstalled = boltmagnet.attached,
                PartPosition = Instrument.transform.position,
                PartRotation = Instrument.transform.localEulerAngles,
                boltsteps = boltmagnet.bolts.Select(b => b.tightness).ToArray(),
                PartBought = Bought
            }, EncryptionKey);
        }
    }
}
