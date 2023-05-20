using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Collections.Concurrent;
using Bot.Infrastructure.DataBaseCore;

namespace Bot.Domain.Entities;

public delegate void StopSessiaDelegate(object source, CoreUser user); 
class Sessia
{
    //Экземпляр делеагата указывающий на то, что надо сделать при истечении таймера
    
    public CoreUser User{get; } 
    public CoreChat Chat{get; }
    private System.Timers.Timer timer; //Таймер
    private StopSessiaDelegate stopSessia;
    public Sessia(CoreUser user,CoreChat chat, StopSessiaDelegate stopSessia)
    {
        this.User = user;
        this.Chat= chat;

        this.timer = new System.Timers.Timer(300000);
        timer.Elapsed += new ElapsedEventHandler((sender, e) => stopSessia(sender, user)); //Присвоение функции на эвент истечение времени
        timer.Enabled = true;
    }
    public void StopSessia(){
        stopSessia.Invoke(timer, User);
    }
    public void ResetTimer(){
        timer.Stop();
        timer.Start();
    }
    
}

//Одновременные сессии
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

    public Sessia GetOrAddSessia(long Id, Func<(CoreUser, CoreChat)> argFunc){
        return sessias.GetOrAdd(Id, (id) => {
                (CoreUser, CoreChat) cortegArg = argFunc();
                return new Sessia(cortegArg.Item1, cortegArg.Item2, StopSessiaAtTimer);
            });
    }
    public void StopSessiaAtUserId(long userId)
    {
        Sessia DelSessia;
        sessias.TryRemove(userId, out DelSessia);
        DelSessia?.StopSessia();
    }

    private async void StopSessiaAtTimer(object source, CoreUser user)
    {
        System.Timers.Timer myTimer = (System.Timers.Timer)source;
        myTimer.Stop();
        myTimer.Dispose();
        Sessia DelSessia;
        sessias.TryRemove(user.Id, out DelSessia);
        Console.WriteLine($"End Sesisa: {DelSessia.User.FirstName}");
        DataBaseHandler.UpdateUser(DelSessia.User);
        ChatHandler.UpdateChat(DelSessia.Chat);
    }
}
