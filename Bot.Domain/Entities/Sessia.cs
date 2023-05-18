using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Collections.Concurrent;

namespace Bot.Domain.Entities;

public delegate void StopSessiaDelegate(object source, ElapsedEventArgs e, CoreUser user); 
class Sessia
{
    //Экземпляр делеагата указывающий на то, что надо сделать при истечении таймера
    
    public CoreUser User{get; } 
    public CoreChat Chat{get; }
    public Buttons CurentButtons{get; } //Меню выбора для пользователя в данный момент веремени
    private System.Timers.Timer timer{get;} //Таймер
    private StopSessiaDelegate stopSessia;
    public Sessia(CoreUser user,CoreChat chat, Buttons curentButtons, StopSessiaDelegate stopSessia)
    {
        this.User = user;
        this.Chat= chat;
        this.CurentButtons = curentButtons;

        this.timer = new System.Timers.Timer(300000);
        timer.Elapsed += new ElapsedEventHandler((sender, e) => stopSessia(sender, e, user)); //Присвоение функции на эвент истечение времени
        timer.Enabled = true;
    }
    
}

//List одновременных сессий
class Sessias
{
    private ConcurrentDictionary<long, Sessia> sessias;
    private List<Buttons> AllButtonsInControllers;

    public Sessias(){
        sessias = new ConcurrentDictionary<long, Sessia>();
        AllButtonsInControllers = new List<Buttons>();
    }

    public void AddButtonInListAllButtons(Buttons buttons){
        AllButtonsInControllers.Add(buttons);
    }
    public Button FindButtonInAllForText(string textButtons){
        foreach(Buttons buttons in AllButtonsInControllers){
            Button returnButton = buttons.FindButtonForText(textButtons);
            if(returnButton != null){
                return returnButton;
            }
        }
        return null;
    }
    public Button FindButtonInAllForFunction(string TextFunction){
        foreach(Buttons buttons in AllButtonsInControllers){
            Button returnButton = buttons.FindButtonForFunction(TextFunction);
            if(returnButton != null){
                return returnButton;
            }
        }
        return null;
    }
    public Sessia GetSessiaAtUserId(long userId)
    {//Получение сессии
        Sessia getSessia;
        sessias.TryGetValue(userId, out getSessia);
        return getSessia;
    }
    public Sessia NewSessia(CoreUser user, CoreChat chat,  Buttons curentButtons)
    {   Sessia newSessia =new Sessia(user, chat, curentButtons, StopSessiaAtTimer);
        this.sessias.Add(newSessia);
        return newSessia;
    }

    public int FindSessiaAtUserId(long userId){
        for (int i = 0; i < this.sessias.Count; i++)
        {
            if (this.sessias[i].User.Id == userId)
                return this.sessias[i]
        }
    }
    private void DelSessiaAtUserId(long userId)
    {
       
        sessias.TryPeek
    }

    private async void StopSessiaAtTimer(object source, ElapsedEventArgs e, CoreUser user)
    {
        System.Timers.Timer myTimer = (System.Timers.Timer)source;
        myTimer.Stop();
        myTimer.Dispose();
        DelSessiaAtUserId(user.Id);
    }
}
