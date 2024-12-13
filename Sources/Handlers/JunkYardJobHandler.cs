using UnityEngine;

using Psycho.Internal;


namespace Psycho.Handlers
{
    internal sealed class JunkYardJobHandler : CatchedComponent
    {
        int m_iCarsNumber = 0;


        internal override void Awaked()
        {
            GameObject Fleetari = GameObject.Find("REPAIRSHOP/LOD/Office/Fleetari");
            for (int i = 1; i < 5; i++)
                StateHook.Inject(Fleetari, "Work", $"Car {i}", _ => m_iCarsNumber += 1);

            StateHook.Inject(gameObject, "Use", "State 1", -1, _ => UsePayMoneyTicket());
        }

        void UsePayMoneyTicket()
        {
            Utils.PrintDebug(eConsoleColors.YELLOW, $"Delivered {m_iCarsNumber} cars in one of time");            
            Logic.PlayerCompleteJob("JUNK_YARD", m_iCarsNumber, $"Delivered {m_iCarsNumber} JunkCars");
            m_iCarsNumber = 0;
        }
    }
}
