
using UnityEngine;

using Psycho.Internal;


namespace Psycho.Handlers
{
    internal sealed class JunkYardJobHandler : CatchedComponent
    {
        int carsCount = 0;


        protected override void Awaked()
        {
            GameObject _fleetari = GameObject.Find("REPAIRSHOP/LOD/Office/Fleetari");
            for (int i = 1; i < 5; i++)
                StateHook.Inject(_fleetari, "Work", $"Car {i}", IncreaseCars);

            StateHook.Inject(gameObject, "Use", "State 1", UsePayMoneyTicket, -1);
        }

        void IncreaseCars() => carsCount += 1;

        void UsePayMoneyTicket()
        {
            Utils.PrintDebug(eConsoleColors.YELLOW, $"Delivered {carsCount} cars in one of time");            
            Logic.PlayerCompleteJob("JUNK_YARD", carsCount, $"Delivered {carsCount} JunkCars");
            carsCount = 0;
        }
    }
}
