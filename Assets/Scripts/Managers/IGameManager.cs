using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGameManager
{
   ManagerStatus status { get; } // завершил ли модуль инициализацию

   void Startup(); // обработка инициализации.

}
