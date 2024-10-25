using UnityEngine;

namespace Psycho
{
    public sealed class JunkYardJobHandler : MonoBehaviour
    {
        bool m_bInstalled = false;
        int m_iCarsNumber = 0;


        void OnEnable()
        {
            if (m_bInstalled) return;

            GameObject Fleetari = GameObject.Find("REPAIRSHOP/LOD/Office/Fleetari");
            for (int i = 1; i < 5; i++)
                StateHook.Inject(Fleetari, "Work", $"Car {i}", () => CheckCars());

            StateHook.Inject(gameObject, "Use", "State 1", -1, () => UsePayMoneyTicket());
            m_bInstalled = true;
        }

        void UsePayMoneyTicket()
        {
            Utils.PrintDebug(eConsoleColors.WHITE, $"Delivered {m_iCarsNumber} cars in one of time");            
            Logic.PlayerCompleteJob("JUNK_YARD", m_iCarsNumber, $"Delivered {m_iCarsNumber} JunkCars");
            m_iCarsNumber = 0;
        }

        void CheckCars() => m_iCarsNumber += 1;
    }
}
