using System;
using UnityEngine;
using System.Collections.Generic;

namespace DestructionEffects
{
	[KSPAddon(KSPAddon.Startup.Flight, false)]
	public class FlamingJoints : MonoBehaviour
	{
		
		public static List<GameObject> flameObjects = new List<GameObject>();
		
		public void Start()
		{
			GameEvents.onPartJointBreak.Add(onPartJointBreak);
			
		}
		
		public void onPartJointBreak(PartJoint partJoint)
		{
			
			if(partJoint.Target!=null && partJoint.Target.PhysicsSignificance != 1)
			{
				Part targetPart = partJoint.Target;
				bool attachFlames = false;
				
				if(targetPart.partInfo.title.Contains("Wing")
					|| targetPart.partInfo.title.Contains("Fuselage") 
					|| targetPart.FindModuleImplementing<ModuleEngines>() 
					|| targetPart.FindModuleImplementing<ModuleEnginesFX>()
					)
				{
					attachFlames = true;	
				}
				else
				{
					foreach(PartResource resource in targetPart.Resources)
					{
						if(resource.resourceName.Contains("Fuel") || resource.resourceName.Contains("Ox"))	
						{
							attachFlames = true;	
						}
					}
				}

				Part hostPart = partJoint.Host; //Exclude struts -- thanks, jkoritzinsky!
				if (hostPart.FindModuleImplementing<CompoundParts.CModuleStrut> () != null) {
					attachFlames = false;
				}

				if(attachFlames)
				{
					GameObject flameObject2 = (GameObject) GameObject.Instantiate(GameDatabase.Instance.GetModel("DestructionEffects/Models/FlameEffect/model"), partJoint.transform.position, Quaternion.identity);
					flameObject2.SetActive(true);
					flameObject2.transform.parent = partJoint.Target.transform;
					flameObject2.AddComponent<FlamingJointScript>();
					foreach(var pe in flameObject2.GetComponentsInChildren<KSPParticleEmitter>())
					{
						if(pe.useWorldSpace)	
						{
							DEGaplessParticleEmitter gpe = pe.gameObject.AddComponent<DEGaplessParticleEmitter>();	
							gpe.part = partJoint.Target;
							gpe.emit = true;
							
						}
					}
				}
			}
			
			
		}
		
		
	}
}

