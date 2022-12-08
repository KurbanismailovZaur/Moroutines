# Moroutines
Moroutines - это библиотека C# написанная для продвинутой работы с корутинами в Unity. 

Unity по умолчанию предоставляет возможность работы с корутинами, однако этот подход имеет недостатки. С помощью этой библиотеки мы попытались обойти эти недостатки 
предлагая вам собственное API для работы с корутинами. Вы можете использовать одновременно как встроенный подход для работы с корутинами, так и нашу библиотеку.

### Почему морутина?
В Unity уже существует класс `Coroutine` для работы с корутинами. В нашей библиотеке корутины называются морутинами (Moroutine - more than coroutine) и представлены классом `Moroutine`. Это позволяет вам легко использовать как чистые корутины, так и более продвинутые морутины.

### Какие преимущества?
Встроенный подход работы с корутинами имеет множество недостатков:
- Корутины B и C не могут одновременно ожидать (yield) корутину A.
- Корутины не хранят информацию о своем состоянии (reseted, running, stopped, completed или destroyed)
- Корутина не может быть приостановлена/сброшена, а затем запущена продолжая свое выполенение.
- Нет возможности добавить задержку перед запуском уже существующей корутины.
- Нет возможности ожидания (yield) паузы или воспроизведения корутины.
- У корутин нет событий изменения их состояний.
- Корутины не умеют возвращать результат.
- Корутины не могут быть сгруппированы для удобного контроля.
- Нет возможности ожидать (yield) выполнение нескольких корутин.
- Корутина не хранит информацию о своем объекте-владельце.
- Нет возможности создать бесхозную корутину.
- Корутины не имеют имен, сложнее фильтруются и тяжелее отлаживаются.
- Игровые объекты не отображают информацию о корутинах, которые с ними связаны.
- И прочие недостатки.

Создавая Moroutines, мы учли эти недостатки и предоставили вам удобное API для работы с корутинами.

### Импорт библиотеки
Вы можете импортировать нашу библиотеку с помощью магазина <a href="https://assetstore.unity.com/packages/tools/utilities/moroutines-214820">Asset Store</a> или скачав Unity-package <a href="https://github.com/KurbanismailovZaur/Moroutines/releases">здесь</a>. 

### Подключение пространства имен
Для работы с морутинами необходимо подключить пространство имен `Redcode.Moroutines`. В этом пространстве находятся все типы данных которые мы создали для работы с морутинами.
```c#
using Redcode.Moroutines;
```
После этого вы можете использовать класс `Moroutine` из данной библиотеки для работы с морутинами.

### Создание морутины
Чтобы создать морутину объявите метод возвращающий перечислитель:
```c#
private IEnumerator TickEnumerator()
{
    while (true)
    {
        yield return new WaitForSeconds(1f);
        print("Tick!");
    }
}
```
В примере выше объявлен метод в котором бесконечно с секундной задержкой выводится текст "Tick!" в консоль Unity. Чтобы создать морутину используйте метод `Moroutine.Create`
```c#
using Redcode.Moroutines;

public class Test : MonoBehaviour
{
    private void Start() => Moroutine.Create(TickEnumerator()); // Создаем морутину.

    private IEnumerator TickEnumerator()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            print("Tick!");
        }
    }
}
```
Метод `Moroutine.Create` создает морутину, но не запускает ее.

### Запуск выполнения морутины
Вы можете запустить морутину вызвав метод `Run`:
```c#
var mor = Moroutine.Create(TickEnumerator());
mor.Run();
```
Или так:
```c#
Moroutine.Create(TickEnumerator()).Run();
```
Или еще короче, используя статический метод `Moroutine.Run`.
```c#
Moroutine.Run(TickEnumerator());
```
> Метод `Moroutine.Run` возвращает запущенную морутину, поэтому вы можете записать ее в переменную.

```c#
using Redcode.Moroutines;

public class Test : MonoBehaviour
{
    private void Start() => Moroutine.Run(TickEnumerator()); // Создаем и запускаем морутину.

    private IEnumerator TickEnumerator()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            print("Tick!");
        }
    }
}
```

Если запустить игру с этим скриптом, то в консоли каждую секунду будут появляться сообщения "Tick!".

![Снимок экрана 2021-08-17 211846](https://user-images.githubusercontent.com/5365111/129779572-6c2d7d0d-0c49-4556-918c-5541c0025d13.jpg)

### Остановка морутины
Чтобы остановить морутину используйте метод `Stop` на объекте морутины.
```c#
var mor = Moroutine.Run(TickEnumerator());  // Запускаем

yield return new WaitForSeconds(1f);        // Ждем 1 секунду
mor.Stop();                                 // Останавливаем
```
### Продолжение морутины
Если необходимо продолжить морутину после остановки, то снова вызовите метод Run на ней.
```c#
var mor = Moroutine.Run(TickEnumerator());  // Запускаем

yield return new WaitForSeconds(1f);        // Ждем 1 секунду
mor.Stop();                                 // Останавливаем

yield return new WaitForSeconds(3f);        // Ждем 3 секунды
mor.Run();                                  // Продолжаем
```

### Завершение морутины
Метод (`TickEnumerator()`) который был передан морутине имеет бесконечный цикл внутри. По этой причине такая морутина никогда не завершится. В противном случае морутина рано или поздно будет завершена. Например:
```c#
private void Start() => Moroutine.Run(DelayEnumerator(1f));

private IEnumerator DelayEnumerator(float delay)
{
    yield return new WaitForSeconds(delay);
    print("Completed!");
}
```

Метод `DelayEnumerator(float delay)` является конечным. Данный метод генерирует некий `IEnumerator` объект представляющий выполнение этого метода. `IEnumerator` объекты не могут быть перезапущены в C#. По этой причине, когда морутина, которой был передан `IEnumerator` объект, завершает выполнение, она автоматически помечается как уничтоженная, что означает, что вы больше не сможете запустить ее снова. Если в вашем коде нет ссылок на уничтоженную морутину, сборщик мусора автоматически удалит ее через некоторое время.

Однако в обявлении метода вы можете заменить `IEnumerator` на `IEnumerable` и в этом случае морутину можно будет перезапускать, чтобы она снова начинала выполнение с самого начала. Именно по этой причине такие морутины не помечаются автоматически как уничтоженные.
```c#
private void Start() => Moroutine.Run(DelayEnumerable(1f));

private IEnumerable DelayEnumerable(float delay) // Обратите внимание что метод теперь возврашает IEnumerable объект.
{
    yield return new WaitForSeconds(delay);
    print("Completed!");
}
```

### Настройка авто-уничтожения
Вы можете управлять автоматическим уничтожением морутины с помощью метода `SetAutoDestroy` или свойства `AutoDestroy`:
```c#
private void Start() => Moroutine.Run(DelayEnumerable(1f)).SetAutoDestroy(true); // <-- настройка авто-уничтожения

private IEnumerable DelayEnumerable(float delay)
{
    yield return new WaitForSeconds(delay);
    print("Completed!");
}
```

В примере морутина использует `IEnumerable` объект, а потому не будет уничтожена автоматически, однако с помощью метода `SetAudoDestroy` мы указали, что после завершения ее надо уничтожить. Точно так же вы можете отменить авто-уничтожение морутины созданной с помощью `IEnumerator` объекта, но это не имеет особого смысла, потому что после завершения такая морутина просто не будет ничего делать, даже если запускать ее снова и снова.

### Ручное уничтожение морутины
Вы можете уничтожить морутину с помощью метода `Destroy`:
```c#
var mor = Moroutine.Run(TickEnumerator());
yield return new WaitForSeconds(3.5f)
mor.Destroy(); // Остановка и уничтожение морутины.
```

> ВАЖНО! Если морутина больше не используется в вашей игре, то она должна быть уничтожена (не важно как, автоматически, или вручную с помощью метода `Destroy`), иначе память не будет освобождена. Также надо не забывать "терять" все ссылки на морутину после уничтожения.

> Используйте `IEnumerator` методы если нет необходимости повторного выполнения морутины. Это избавит вас от лишней настройки авто-уничтожения. В противном случае вы должны не забыть уничтожить морутину.

### Перезапуск морутины
Вы можете перезапустить морутину (начать ее выполнение с самого начала), для этого используйте метод `Reset`.
```c#
private IEnumerator Start()
{
    var mor = Moroutine.Run(TimerEnumerable());
    yield return new WaitForSeconds(3.5f);    // Ждем 3.5 секунды..

    mor.Reset();    // Останавливаем и обнуляем морутину (возвращаем в исходное состояние).
    mor.Run();      // Запускаем повторно.
}

private IEnumerable TimerEnumerable()
{
    var seconds = 0;

    while (true)
    {
        yield return new WaitForSeconds(1f);
        print(++seconds);
    }
}
```

![image](https://user-images.githubusercontent.com/5365111/161645791-dcf234b7-bc08-480a-b534-da546b1be91f.png)

Обратите внимание, что вызов метода `Reset` сбрасывает состояние морутины и останавливает ее. Это значит, что вы сами должны позаботиться о ее дальнейшем запуске. Методы `Run`, `Stop` и `Reset` возвращают морутину, которой они принадлежат, это позволяет сцепить несколько вызовов методов друг с другом и сократить код.
```c#
mor.Reset().Run();
```

Или еще короче:
```c#
mor.Rerun();
```

После выполнения морутины вы также можете вызвать метод `Rerun` на ней, чтобы снова запустить ее, однако вместо этого лучше используйте метод `Run`, он имеет параметр `rerunIfCompleted`, который по умолчанию равен `true` и отвечает за автоматический перезапуск морутины если она уже завершена.

### Состояние морутины
Вы можете проверить состояние морутины с помощью следующих свойств:
- `IsReseted` - сброшено ли состояние морутины до начального?
- `IsRunning` - запущена ли морутина?
- `IsStopped` - остановлена ли морутина?
- `IsCompleted` - завершена ли морутина?
- `IsDestroyed` - уничтожена ли морутина?
- `CurrentState` - возвращает перечисление, которое представляет одно из вышеперечисленных состояний.

Первые четыре возвращают булево значение, представляющее соответствующее состояние. Пример:
```c#
var mor = Moroutine.Run(CountEnumerable());
print(mor.IsRunning);
```

### События и методы подписки
Морутины имеют следующие события:
- `Reseted` - срабатывает когда морутина сбрасывается в начальное состояние.
- `Running` - срабатывает сразу после вызова метода `Run`.
- `Stopped` - срабатывает только когда морутина остановилась.
- `Completed` - срабатывает когда морутина завершилась.
- `Destroyed` - срабатывает когда морутина уничтожена.

Вы можете подписаться на любое из этих событий когда это необходимо. Метод-подписчик должен соответствовать следующей сигнатуре:
```c#
void EventHandler(Moroutine moroutine);
```
В параметр `moroutine` будет подставлена морутина вызвавшая событие.
```c#
var mor = Coroutine.Run(CountEnumerable());
mor.Completed += mor => print("Completed");
```
Вы также можете быстро подписаться на нужное событие с помощью следющих методов:
- OnReseted - подписка на обнуление.
- OnRunning - подписка на запуск.
- OnStopped - подписка на остановку.
- OnCompleted - подписка на завершение.
- OnDestroyed - подписка на уничтожение.

```c#
var mor = Moroutine.Run(CountEnumerable());
mor.OnCompleted(c => print("Completed"));
```
Почти все методы морутин возвращают морутину на которой они вызываются, поэтому вы можете формировать длинные цепочки вызовов, например такие:
```c#
Moroutine.Create(CountEnumerable()).OnCompleted(c => print("Completed")).Run();
```

### Ожидание морутины
Если необходимо подождать (yield) определенное состояние морутины, то используйте следующие методы:
- `WaitForComplete` - возвращает объект для ожидания завершения.
- `WaitForStop` - возвращает объект для ожидания остановки.
- `WaitForRun` - возвращает объект для ожидания запуска.
- `WaitForReset` - возвращает объект для ожидания обнуления.
- `WaitForDestroy` - возвращает объект для ожидания уничтожения.

Например так:
```c#
var mor = Moroutine.Run(CountEnumerable());

yield return mor.WaitForComplete();             // ждем пока морутина завершится
print("Awaited");                               // выводим текст после завершения морутины
```
Или так:
```c#
yield return Moroutine.Run(CountEnumerable()).WaitForComplete();
print("Awaited");
```
Во встроенном движке корутин нельзя чтобы корутины B и C одновременно ожидали завершения корутины A, иначе вы получите предупреждение в окне консоли:
```c#
private void Start()
{
    var coroutine = StartCoroutine(SomeEnumerator());    // корутина A, имитирует некий процесс
    StartCoroutine(WaitEnumerator(coroutine));           // корутина B, ждет корутину A, все ок
    StartCoroutine(WaitEnumerator(coroutine));           // корутина C, ждет корутину A, ошибка
}

private IEnumerator SomeEnumerator()
{
    yield return new WaitForSeconds(3f); // имитируем некий процесс выполнения..
}

private IEnumerator WaitEnumerator(Coroutine coroutine)
{
    yield return coroutine; // ждем полученную корутину
    print("Awaited");
}
```

![image](https://user-images.githubusercontent.com/5365111/129798948-97ad275f-1c06-4983-83a2-ab293673347d.png)

Как видите, это действительно так, однако с морутинами такой проблемы нет, вы можете создавать сколько угодно морутин, которые будут ожидать какие угодно другие морутины!
```c#
private void Start()
{
    var mor = Moroutine.Run(SomeEnumerable()); // Морутина A
    Moroutine.Run(WaitEnumerable(mor)); // Морутина B, ждет морутину A, все ок
    Moroutine.Run(WaitEnumerable(mor)); // Морутина C, ждет морутину A, все ок
}

private IEnumerable SomeEnumerable()
{
    yield return new WaitForSeconds(3f); // имитируем некий процесс выполнения..
}

private IEnumerable WaitEnumerable(Moroutine moroutine)
{
    yield return moroutine.WaitForComplete(); // ждем полученную морутину
    print("Awaited");
}

```

![image](https://user-images.githubusercontent.com/5365111/129799598-7ebef6dc-a78b-4174-858a-07338e400a3f.png)

### Работа с несколькими морутинами
Вы можете создавать несколько морутин сразу с помощью методов `Create` и `Run`.
```c#
private void Start()
{
    List<Moroutine> mors = Moroutine.Run(TickEnumerable("mor1", 1), TickEnumerable("mor2", 2));
}

private IEnumerable TickEnumerable(string prefix, int count)
{
    for (int i = 0; i < count; i++)
    {
        yield return new WaitForSeconds(1f);
        print($"{prefix}: Tick!");
    }
}
```

![image](https://user-images.githubusercontent.com/5365111/161760852-cc572426-7c66-4ce8-9bc9-590f45bdf18d.png)

В этом случае метод будет возвращать список созданных морутин.

#### Ожидание завершения нескольких морутин
Вы также можете ожидать несколько морутин сразу с помощью объекта класса `WaitForAll`.

```c#
private IEnumerator Start()
{
    var mors = Moroutine.Run(TickEnumerable("mor1", 1), TickEnumerable("mor2", 2));
    yield return new WaitForAll(mors);

    print("All awaited!");
}

private IEnumerable TickEnumerable(string prefix, int count)
{
    for (int i = 0; i < count; i++)
    {
        yield return new WaitForSeconds(1f);
        print($"{prefix}: Tick!");
    }
}
```

![image](https://user-images.githubusercontent.com/5365111/161756110-8862133c-5991-42c9-8eb2-5f8e9588dd36.png)

Конструктор класса `WaitForAll` имеет следующие перегрузки:
```c#
WaitForAll(params Moroutine[]);
WaitForAll(IEnumerator[]);
WaitForAll(IEnumerable<IEnumerator>);
```

#### Ожидание завершения хотя бы одной из нескольких морутин
С помощью класса `WaitForAny` можно ожидать выполнение хотя бы одной морутины из указанного списка.
```c#
private IEnumerator Start()
{
    var tickMor1 = Moroutine.Run(TickEnumerable("mor1", 1));
    var tickMor2 = Moroutine.Run(TickEnumerable("mor2", 2));

    yield return new WaitForAny(tickMor1, tickMor2);
    print("Any awaited!");
}

private IEnumerable TickEnumerable(string prefix, int count)
{
    for (int i = 0; i < count; i++)
    {
        yield return new WaitForSeconds(1f);
        print($"{prefix}: Tick!");
    }
}
```

![image](https://user-images.githubusercontent.com/5365111/161758650-3305b167-cb34-4f0d-b90a-05dffbcf9303.png)

Конструктор класса `WaitForAny` имеет следующие перегрузки:
```c#
WaitForAny(params Moroutine[]);
WaitForAny(IEnumerator[]);
WaitForAny(IEnumerable<IEnumerator>);
```

### Результат морутины
Морутины умеют хранить результат выполнения. Для этого существует свойство `LastResult`. Данное свойство хранит в себе объект возвращаенный последним выполненным оператором `yield return`.

```c#
private IEnumerator Start()
{
    var mor = Moroutine.Run(_owner, GenerateSomeResultEnumerable());
    yield return mor.WaitForComplete(); // ждем морутину.
    
    print(mor.LastResult); // выводим ее последний результат.
}

private IEnumerable GenerateSomeResultEnumerable()
{
    yield return new WaitForSeconds(3f); // симулируем некий процесс..
    yield return "Hello from moroutine!"; // а это будет последним результатом морутины.
}
```

![image](https://user-images.githubusercontent.com/5365111/141380042-0a009674-c783-4c24-8083-15acb3d6513f.png)

Иногда это бывает очень удобно!

### Бесхозные морутины
До сих пор мы с вами изучали как создавать бесхозные морутины. Бесхозная морутина - это морутина, которая не привязана ни к одному игровому объекту. Выполнение такой морутины может быть прервано только с помощью методов `Stop`, `Reset` или `Destroy`. Бесхозные морутины продолжают свое существование и выполнение даже если вы загрузите новую сцену в игре, поэтому к ним нужно быть более внимательными. Под капотом, бесхозная морутина запускается на игровом объекте, который расположен в DontDestroyOnLoad сцене.

### Морутины и их владельцы
Вы можете связать морутину с любым игровым объектом, то есть сделать его владельцем морутины. Это значит, что выполнение морутины будет возможным только в случае если объект-владелец активен, в противном случае морутина будет остановлена и вы не сможете перезапустить ее или продолжить до тех пор пока объект-владелец не станет активен. Попытка запуска морутины на неактивном объекте-владельце сгенерирует исключение. Если объект-владелец вновь активен, то вы можете продолжить выполнение морутины используя метод `Run`.

Чтобы указать хозяина морутины укажите его первым параметром в методах `Moroutine.Create` или `Moroutine.Run`.
```c#
var mor = Moroutine.Run(gameObject, CountEnumerable()); // gameObject - это хозяин морутины
```

Вместо объекта-владельца можно передать любой его компонент. Результат будет таким же.

```c#
var mor = Moroutine.Run(this, CountEnumerable()); // this - это ссылка на текущий компонент по которому будет найден хозяин морутины
```

Также вы можете использовать методы `SetOwner` и `MakeUnowned` чтобы установить другого владельца или сделать морутину бесхозной.

```c#
var mor = Moroutine.Run(gameObject, CountEnumerable()); // запускаем морутину

yield return new WaitForSeconds(1f); // ждем секунду
mor.SetOwner(otherGameObject); // установили другого владельца

yield return new WaitForSeconds(1f); // ждем секунду
mor.MakeUnowned(); // сделали бесхозной
```

> Используйте `this` вместо `gameObject`, так как это короче. 

> Вы также можете использовать `mor.SetOwner((GameObject)null)`, чтобы сделать морутину бесхозной.

> При вызове `SetOwner` в процессе выполнения морутины, ее владелец незаметно будет подменен на указанного.

### Компонент `MoroutinesOwner`
Всякий раз, когда какой-либо игровой объект указывается в качестве владельца морутины, к нему автоматически добавляется компонент `MoroutinesOwner`. Данный компонент всегда хранит актуальный список морутин которыми он владеет, а также отвечает за их деактивацию когда отключают или удаляют игровой объект.

![image](https://user-images.githubusercontent.com/5365111/182028695-ddae59c7-63f1-4678-9cee-97ee8405a395.png)

Напротив кнопки "Show Moroutines" указано количество актуальных морутин, которыми владеет данный игровой объект. Нажатие на эту кнопку отобразит подробную информацию о всех актуальных морутинах (их индексы в списке, имена, состояние и последний результат):

![image](https://user-images.githubusercontent.com/5365111/182029026-5cbbbf98-65ca-40a2-8265-d854bea06f17.png)

Если вам необходимо получить хозяина морутины, используйте ее свойство `Owner`.
```c#
var mor = Moroutine.Run(gameObject, CountEnumerable());
print(mor.Owner.name);
```

`Owner` - это ссылка на компонент `MoroutinesOwner` владельца морутины. У бесхозной морутины `Owner` равен `null`. Если вам необходимо узнать имеет ли морутина владельца или является бесхозной, то используйте булево свойство `IsOwned`.

> Компонент `MoroutinesOwner` будет существовать до тех пор, пока у него есть хотя бы одна актуальная морутина. Уничтоженная морутина не является актуальной.

### Имена морутин
Вы могли заметить, что у компонента `MoroutinesOwner` в столбце "Names" списка морутин все имена обозначены как "[noname]". Это означает, что вы не указывали специального имени этим морутинам. Чаще всего ваши морутины будут оставаться безымянными, однако иногда в целях отладки удобно дать какой-либо из них имя. Для этого используйте метод `SetName` или свойство `Name` у морутины:
```c#
Moroutine.Run(_owner, TurningEnumerator()).SetName("Turning");
Moroutine.Run(_owner, MovingEnumerator(2f)).Name = "Moving";
Moroutine.Create(_owner, JumpingEnumerator()).SetName("Jumping");
```

![image](https://user-images.githubusercontent.com/5365111/182029752-c7acb6f0-9224-426e-a575-538c0716eeec.png)

### Получение всех морутин владельца
Имея компонент `MoroutinesOwner` вы можете получить список его актуальных морутин с помощью свойства `Moroutines`:
```c#
var mor = Moroutine.Run(_owner, TurningEnumerator());
Moroutine.Run(_owner, MovingEnumerator(2f));
Moroutine.Create(_owner, JumpingEnumerator());

var moroutines = mor.Owner.Moroutines; // получаем readonly-список морутин
```
В примере выше создаются 3 морутины у которых один и тот же владелец. С помощью свойства `Owner` мы получили владельца, а затем и все его морутины. Свойство `Moroutines` возвращает новый экземпляр `ReadOnlyCollection`, так что вы не сможете менять оригинал.

Другой способ получить все морутины игрового объекта заключается в использовании метода расширения `GetMoroutines()`. Не забудбте подключить пространство имен `Redcode.Moroutines.Extensions`:
```c#
// ...
using Redcode.Moroutines.Extensions;
// ...

Moroutine.Run(this, TickEnumerable(1), TickEnumerable(2)); // запустили 2 морутины на текущем игровом объекте
var mors = gameObject.GetMoroutines(); // получили все морутины текущего игрового объекта
```

Вы также можете использовать маску состояния чтобы отфильтровать морутины.
```c#
var mors = gameObject.GetMoroutines(Moroutine.State.Stopped | Moroutine.State.Running); // получение только тех морутин владельца, которые находятся в состоянии Stoped или Runnning
```

### Объект `MoroutinesExecuter`
Перед запуском вашей игры в сцене будет создан объект `MoroutinesExecuter`, который будет изолирован в `DontDestroyOnLoad` сцену и скрыт в ней, так что вы не заметите его. Вы также не сможете получить доступ к этому классу из кода. Данный объект является владельцем всех бесхозных морутин. 

### Получение всех бесхозных морутин
Используйте статический метод `Moroutine.GetUnownedMoroutines` для получения бесхозных морутин. Также можно использовать маску состояния.

```c#
var mors = Moroutine.GetUnownedMoroutines(Moroutine.State.Running); // получение всех бесхозных запущенных морутин
```

### Настройки Moroutines
Окно настроек находится в меню `Window` > `Moroutines` > `Settings` и состоит из следующих пунктов:
- Hide Moroutines Executer - нужно ли скрывать владельца всех бесхозных морутин в редакторе или нет?

![image](https://user-images.githubusercontent.com/5365111/182030856-570d044c-7172-4b9a-a916-a8bf0382f9d4.png)

### Группировка морутин с помощью `MoroutinesGroup`
Иногда бывает удобно сгруппировать морутины в один объект и управлять ими всеми через него. Для этого используйте класс `MoroutinesGroup`.
```c#
var mor1 = Moroutine.Create(TickEnumerator(1));
var mor2 = Moroutine.Create(TickEnumerator(3));

var group = new MoroutinesGroup(mor1, mor2);
group.Run();
```

Вы также можете передать в конструктор список морутин:
```c#
var mors = Moroutine.Create(TickEnumerator(1), TickEnumerator(3));

var group = new MoroutinesGroup(mors);
group.Run();
```

Более того, вы можете подключить пространство имен `Redcode.Moroutines.Extensions` и использовать метод `ToMoroutinesGroup()` на коллекции морутин, чтобы создать из нее группу:
```c#
var group = Moroutine.Create(TickEnumerator(1), TickEnumerator(3)).ToMoroutinesGroup();
group.Run();
```

#### Управление группой
Используйте методы `Run`, `Stop`, `Reset`, `Rerun` и `Destroy` чтобы управлять всей группой. Вызовы данных методов по сути просто вызывают эти методы на каждой морутине группы:
```c#
var group = Moroutine.Create(TickEnumerator(1), TickEnumerator(3)).ToMoroutinesGroup();

group.Run();
yield return new WaitForSeconds(1f);

group.Rerun();
```

#### Изменение состава группы 
Список морутин из которых состоит группа представлен свойством `Moroutines`. Это обычный список, вы можете работать с ним так же как и с любым другим списком:
```c#
group.Moroutines.Add(mor7);
group.Moroutines.RemoteAt(4);
```

#### Установка владельца для всех морутин в группе
Для установки владельца всем морутинам группы используйте методы `SetOwner` или `MakeUnowned`. Также вы можете использовать свойство `Owner`, чтобы получить владельца всех морутин, однако если есть хотя бы одна морутина, чей владелец отличается от остальных, то данное свойство вернет значение `null`.
```c#
var group = Moroutine.Run(TickEnumerator(1), TickEnumerator(3)).ToMoroutinesGroup();
group.SetOwner(_owner);

print(group.Owner.name);
```

#### Состояние всех морутин
Вы можете использовать любое из следующих свойств для определения соответствуют ли все морутины группы этому состоянию:
- `IsReseted` - все морутины сброщены.
- `IsRunning` - все морутины запущены.
- `IsStopped` - все морутины остановлены.
- `IsCompleted` - все морутины завершены.
- `IsDestroyed` - все морутины помечены как уничтоженные.
- `IsOwned` - все морутины имеют владельца.

#### Уничтожение всех морутин и настройка автоуничтожения
Используйте свойство `AutoDestroy` или метод `SetAutoDestroy` чтобы считать/установить поведение автоуничтожения для всей группы. Если нужно принципиально уничтожить все морутины, используйте метод `Destroy`.

#### События группы
Когда вы вызываете методы управления группой (`Run`, `Stop` и прочие), то в группе срабатывает соответствующее событие. Поддерживаются следующие события:
- `Reseted` - все морутины группы сброшены.
- `Running` - все морутины группы запущены.
- `Stopped` - все морутины группы остановлены.
- `Destroyed` - все морутины группы уничтожены.

Вы можете быстро подписаться на эти события с помощью методов `OnReseted`, `OnRunning`, `OnStopped` и `OnDestroyed` соответственно:
```c#
var group = Moroutine.Create(TickEnumerator(1), TickEnumerator(3)).ToMoroutinesGroup();
group.OnStopped(g => print("Stopped")).Run(); // подписали на событие остановки всей группы

yield return new WaitForSeconds(2f);

group.Stop(); // останавливаем группу, сработает событие Stopped
```

#### Поиск бесхозных морутин в группе
Используйте метод `GetUnownedMoroutines` (можно указать маску поиска) чтобы получить список бесхозных морутин в группе.

#### Ожидание событий группы
Вы можете ожидать нужное вам событие группы. Для этого используйте один из следующих методов:
- `WaitForComplete` - ожидать пока все морутины группы окажутся завершенными.
- `WaitForStop` - ожидать пока все морутины группы окажутся остановленными.
- `WaitForRun` - ожидать пока все морутины группы окажутся запущенными.
- `WaitForReset` - ожидать пока все морутины группы окажутся сброшенными.
- `WaitForDestroy` - ожидать пока все морутины группы окажутся отмеченными как уничтоженные.

В примере ниже запустим морутину, которая будет ждать пока группа не начнет выполнение:
```c#
private IEnumerator Start()
{
    var group = Moroutine.Create(TickEnumerator(1), TickEnumerator(3)).ToMoroutinesGroup(); // создаем (не запускаем) 2 морутины
    Moroutine.Run(WaitEnumerator(group)); // запускаем морутину, которая будет ждать пока группа не начнет выполняться

    yield return new WaitForSeconds(2f); // ждем 2 секунды

    group.Run(); // запускаем группу
}

private IEnumerable TickEnumerator(int count)
{
    for (int i = 0; i < count; i++)
    {
        yield return new WaitForSeconds(1f);
        print(i);
    }
}

private IEnumerable WaitEnumerator(MoroutinesGroup group)
{
    yield return group.WaitForRun(); // ждем пока группа не начнет выполнение
    print($"Run awaited! (Time.time = {Time.time})");
}
```

![image](https://user-images.githubusercontent.com/5365111/182036044-c84069ec-b120-402b-8fee-44a4b07ac9c6.png)

Как видите, морутина `WaitEnumerator` подождала пока группа не начала выполнение.

### Вспомогательный класс `Routines`
Статический класс `Routines` хранит в себе наиболее часто используемые методы для организации логики выполнения морутин. Все методы генерируют и возвращают объект `IEnumerable`, который можно использовать подставляя в другие методы. В частности имеются следующие методы:
- `Delay` - добавляет временную задержку перед выполнением метода.
- `FrameDelay` - добавляет кадровую задержку перед выполнением метода.
- `Repeat` - повторяет выполнение метода указанное количество раз.
- `Wait` - ожидает выполнение объектов `YieldInstruction` и `CustomYieldInstruction`.

Пример с `Delay`:
```c#
private void Start() => Moroutine.Run(Routines.Delay(1f, CountEnumerable()));

private IEnumeraable CountEnumerable()
{
    for (int i = 1; i <= 3; i++)
    {
        yield return new WaitForSeconds(1f);
        print(i);
    }
}
```
В этот примере используется метод `Delay`, который добавляет секундную задержку перед выполнением перечислителя `CountEnumerator`, для этого используется строка `Routines.Delay(1f, CountEnumerable())`. Как уже говорилось выше, все методы класса `Routines` возвращают объект `IEnumerable`, поэтому, чтобы сделать из результата склейки методов `Delay` и `CountEnumerable` морутину, нужно подставить его в метод `Moroutine.Run`.

Метод `Delay` умеет также работать с Action-методами, что по сути дает вам возможность быстро организовать отложенное выполнение нужного вам метода, например:
```c#
private void Start() => Moroutine.Run(Routines.Delay(1f, () => print("Delayed print!")));
```

или
```c#
private void Start() => Moroutine.Run(Routines.Delay(1f, () => Welcome("Andrew", 29)));

private void Welcome(string name, int age) => print($"Hello {name}, you are {age} years old!");
```

![image](https://user-images.githubusercontent.com/5365111/129882932-0ade0009-9599-4226-9567-046fa6a91762.png)

Как видите это очень удобно и сокращает код.
> Данные методы могут работать как с `IEnumerable` так и с `IEnumerator` объектами, однако, если вы планируете перезапуск ваших перечислителей, то необходимо использовать `IEnumerable` объекты.

Метод `FrameDelay` добавляет кадровую задежку перед выполнение перечислителя. К примеру, если нужно подождать 1 игровой кадр, а затем выполнить код перечислителя, то это будет выглядеть так:
```c#
private void Start() => Moroutine.Run(Routines.FrameDelay(1, () => print("1 frame skipped!")));
```
Данный метод также как и метод `Delay` умеет работать с Action-методами.

Метод `Repeat` повторяет указанный перечислитель указанное количество раз. Если вам нужно бесконечное повторение выполнения перечислителя, то укажите -1 в качестве параметра `count` метода `Repeat`. Пример:
```c#
private void Start() => Moroutine.Run(Routines.Repeat(3, WaitAndPrintEnumerator()));

private IEnumerable WaitAndPrintEnumerator()
{
    yield return new WaitForSeconds(1f);
    print("Hello!");
}
```
В результате в консоль 3 раза через каждую секунду будет выведен текст "Hello!".

![image](https://user-images.githubusercontent.com/5365111/129884060-ffa723c8-c7c7-4fe7-8400-e7b3dc37ee40.png)

Вы можете комбинировать методы `Delay`, `FrameDelay` и `Repeat` между собой, например, если нужно выполнить некую функцию 3 раза с задержкой в 1 секунду, то это будет выглядеть так:
```c#
private void Start() => Moroutine.Run(Routines.Repeat(3, Routines.Delay(1f, () => print(Time.time))));
```

![image](https://user-images.githubusercontent.com/5365111/129884562-3ad492a7-de74-466f-be0e-3cbe27654555.png)

Такое вложение методов друг в друга может быть неограниченным.

Метод `Wait` позволяет быстро обернуть `YieldInstrution` или `CustomYieldInstruction` объект в `IEnumerable`, который просто будет ждать их выполнения. Например, если вы хотите обернуть `YieldInstruction` объект в корутину, чтобы в дальнейшем следить за состоянием выполнения `YieldInstruction` через эту корутину, то вы можете написать такой код:
```c#
var moroutine = Moroutine.Run(Routines.Wait(instruction));
```
Где `instruction` это объект класса `YieldInstruction`.

### Расширения
В пространстве имен `Redcode.Moroutines.Extensions` находятся методы расширения для классов `YieldInstruction` и `CustomYieldInstruction`. Эти методы позволяют быстро конвертировать `Moroutine`, `YieldInstruction` и `CustomYieldInstruction` друг в друга. Например:
```c#
var delayMoroutine = Moroutine.Run(Routines.Delay(1f, () => print("Converting")));  // Создали морутину

var yieldInstruction = delayMoroutine.WaitForComplete();                            // Получили YieldInstruction объект
var customYieldInstruction = yieldInstruction.AsCustomYieldInstruction();           // YieldInstruction конвертировали в CustomYieldInstruction
var moroutine = customYieldInstruction.AsMoroutine();                               // CustomYieldInstruction конвертировали в новый объект Moroutine
```

Такое преобразование скорее всего редко вам понадобится, однако возможность имеется.

Ну а пока что это все, теперь вы готовы использовать морутины!
