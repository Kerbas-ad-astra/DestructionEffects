using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DestructionEffects
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class FlamingJoints : MonoBehaviour
    {
        public static List<GameObject> FlameObjects = new List<GameObject>();

        public static List<string> PartTypesTriggeringUnwantedJointBreakEvents = new List<string>(9)
        {
            "decoupler",
            "separator",
            "docking",
            "grappling",
            "landingleg",
            "clamp",
            "gear",
            "wheel",
            "Turret",
            "MissileLauncher"

        };
        //1553 void OnPartJointBreak(PartJoint j, float breakForce)
        public void Start()
        {
            GameEvents.onPartJointBreak.Add(OnPartJointBreak);
        }

        public void OnPartJointBreak(PartJoint partJoint, float breakForce)
        {
            if (partJoint.Target == null)
            {
                return;
            }
            if (partJoint.Target.PhysicsSignificance == 1)
            {
                return;
            }
            if (!ShouldFlamesBeAttached(partJoint))
            {
                return;
            }
            // if part has module missile turret  part.FindModuleImplementing<ModuleMissileTurret>())
            //  if (GameObject.FindModuleImplementing<ModuleMissileTurret>())
            // {
            //     return;
            //  }

            AttachFlames(partJoint);
        }

        private static void AttachFlames(PartJoint partJoint)
        {
            var flameObject2 =
                (GameObject)
                    Instantiate(
                        GameDatabase.Instance.GetModel("DestructionEffects/Models/FlameEffect/model"),//Hard address for flame model
                        partJoint.transform.position,
                        Quaternion.identity);

            flameObject2.SetActive(true);
            flameObject2.transform.parent = partJoint.Target.transform;
            flameObject2.AddComponent<FlamingJointScript>();

            foreach (var pe in flameObject2.GetComponentsInChildren<KSPParticleEmitter>())
            {
                if (!pe.useWorldSpace) continue;

                var gpe = pe.gameObject.AddComponent<DeGaplessParticleEmitter>();
                gpe.Part = partJoint.Target;
                gpe.Emit = true;
            }
        }

        private static bool ShouldFlamesBeAttached(PartJoint partJoint)
        {
            if (IsPartHostTypeAJointBreakerTrigger(partJoint.Host.name.ToLower()))
            {
                return false;
            }

            var part = partJoint.Target;//SM edit for DE on ships and ship parts, adding bow, hull, stern, superstructure

            if (part.partInfo.title.Contains("Wing") || part.partInfo.title.Contains("Fuselage") || part.partInfo.title.Contains("Bow") || part.partInfo.title.Contains("Stern") || part.partInfo.title.Contains("Hull") || part.partInfo.title.Contains("Superstructure") || part.FindModuleImplementing<ModuleEngines>() || part.FindModuleImplementing<ModuleEnginesFX>())/*|| part.partInfo.title.Contains("Turret") */
            {
                return true;
            }

            return
                part.Resources//SM edit adding EC Ammo and Cannonshells
                    .Any(resource => resource.resourceName.Contains("Fuel") || resource.resourceName.Contains("Ox") || resource.resourceName.Contains("Elec") || resource.resourceName.Contains("Amm") || resource.resourceName.Contains("Cann"));
        }

        private static bool IsPartHostTypeAJointBreakerTrigger(string hostPartName)
        {
            return PartTypesTriggeringUnwantedJointBreakEvents.Any(x => hostPartName.Contains(x));
        }
    }
}