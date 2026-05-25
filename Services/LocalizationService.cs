using Microsoft.JSInterop;

namespace KppBlazor.Services;

public class LocalizationService
{
    private readonly IJSRuntime _js;
    public string CurrentLang { get; private set; } = "ru";
    public event Action? OnChange;

    // Словари: [lang][key]
    private static readonly Dictionary<string, Dictionary<string, string>> Translations = new()
    {
        ["ru"] = new()
        {
            // Общие
            ["app.title"] = "КПП — Контроль гостей",
            ["app.subtitle"] = "Система учёта посетителей",
            ["nav.history"] = "История",
            ["nav.register"] = "Новая заявка",
            ["nav.users"] = "Пользователи",
            ["nav.roles"] = "Роли и права",
            ["nav.menu"] = "Меню",
            ["nav.admin"] = "Администрирование",
            ["nav.stats"] = "Статистика",
            ["btn.login"] = "Войти",
            ["btn.logout"] = "Выйти",
            ["btn.refresh"] = "Обновить",
            ["btn.create"] = "Создать",
            ["btn.save"] = "Сохранить",
            ["btn.cancel"] = "Отмена",
            ["btn.delete"] = "Удалить",
            ["btn.close"] = "Закрыть",
            ["lbl.login"] = "Логин",
            ["lbl.password"] = "Пароль",
            ["lbl.search"] = "ФИО, паспорт, номер авто...",
            ["lbl.username"] = "Логин",
            ["lbl.displayName"] = "Отображаемое имя",
            ["lbl.role"] = "Роль",

            // Дашборд
            ["dashboard.title"] = "Панель управления",
            ["dashboard.subtitle"] = "Обзор активности КПП",
            ["dashboard.inside"] = "Внутри",
            ["dashboard.today"] = "Сегодня",
            ["dashboard.week"] = "За неделю",
            ["dashboard.pending"] = "Ожидается",
            ["dashboard.exited"] = "Вышли сегодня",
            ["dashboard.total"] = "Всего",
            ["dashboard.recent"] = "Последние действия",
            ["dashboard.waiting"] = "Ожидающие гости",
            ["dashboard.visits"] = "Визиты за неделю",
            ["dashboard.peaks"] = "Часы пик",
            ["dashboard.noActions"] = "Пока нет действий",
            ["dashboard.noPending"] = "Нет ожидающих гостей",
            ["dashboard.hour"] = "ч",

            // История
            ["history.title"] = "История посещений",
            ["history.subtitle"] = "Учёт въезда и выезда гостей",
            ["history.guest"] = "Гость",
            ["history.purpose"] = "Цель / Принимает",
            ["history.car"] = "Авто",
            ["history.entry"] = "Въезд",
            ["history.exit"] = "Выезд",
            ["history.time"] = "Время",
            ["history.status"] = "Статус",
            ["history.docs"] = "Документы",
            ["history.actions"] = "Действия",
            ["history.all"] = "Все",
            ["history.pending"] = "Ожидается",
            ["history.inside"] = "Внутри",
            ["history.exited"] = "Вышел",
            ["history.found"] = "Найдено:",
            ["history.today"] = "Сегодня",
            ["history.week"] = "7 дней",
            ["history.reset"] = "Сбросить",
            ["history.from"] = "С:",
            ["history.to"] = "По:",
            ["history.noRecords"] = "Нет записей по заданным фильтрам",
            ["history.loading"] = "Загрузка...",

            // Регистрация
            ["register.title.admin"] = "Новая заявка",
            ["register.title.kpp"] = "Регистрация гостя",
            ["register.title.user"] = "Создать заявку",
            ["register.purpose"] = "Цель визита",
            ["register.host"] = "Принимающее лицо",
            ["register.plannedDate"] = "Плановая дата",
            ["register.transport"] = "Транспорт",
            ["register.optional"] = "необязательно",
            ["register.carBrand"] = "Марка",
            ["register.carPlate"] = "Гос. номер",
            ["register.guestData"] = "Данные гостя",
            ["register.oneGuest"] = "Один гость",
            ["register.group"] = "Группа",
            ["register.fullName"] = "ФИО",
            ["register.dob"] = "Дата рождения",
            ["register.passport"] = "Серия и номер",
            ["register.nationality"] = "Гражданство",
            ["register.passportScan"] = "Скан паспорта",
            ["register.permitDoc"] = "Пропускной документ",
            ["register.loaded"] = "Загружен",
            ["register.addGuest"] = "Добавить гостя",
            ["register.guest"] = "Гость",
            ["register.create"] = "Создать заявку",
            ["register.submitKpp"] = "Зарегистрировать",
            ["register.submitUser"] = "Отправить заявку",
            ["register.saving"] = "Сохранение...",
            ["register.registered"] = "Гость зарегистрирован!",
            ["register.created"] = "Заявка создана!",
            ["register.guestsRegistered"] = "гостей успешно зарегистрированы",
            ["register.guestsAdded"] = "гостей успешно добавлены",
            ["register.guestRegistered"] = "Гость успешно зарегистрирован",
            ["register.guestAdded"] = "Гость успешно добавлен в список ожидания",
            ["register.toHistory"] = "В историю",
            ["register.newRequest"] = "Новая заявка",
            ["register.banner.admin"] = "Заявка создаётся со статусом «Ожидается».",
            ["register.banner.kpp"] = "Гость будет зарегистрирован со статусом «Внутри».",
            ["register.banner.user"] = "Ваша заявка будет передана на КПП.",

            // Пользователи
            ["users.title"] = "Пользователи",
            ["users.createUser"] = "Создать пользователя",
            ["users.user"] = "Пользователь",
            ["users.status"] = "Статус",
            ["users.created"] = "Создан",
            ["users.actions"] = "Действия",
            ["users.active"] = "Активен",
            ["users.blocked"] = "Заблокирован",
            ["users.block"] = "Заблок.",
            ["users.unblock"] = "Разблок.",
            ["users.password"] = "Пароль",
            ["users.newPassword"] = "Новый пароль",
            ["users.chooseRole"] = "— выберите —",

            // Роли
            ["roles.title"] = "Роли и права",
            ["roles.createRole"] = "Создать роль",
            ["roles.name"] = "Название",
            ["roles.systemName"] = "Системное имя",
            ["roles.requests"] = "Заявки",
            ["roles.history"] = "История",
            ["roles.entryExit"] = "Вход/Выход",
            ["roles.actions"] = "Действия",
            ["roles.system"] = "системная",
            ["roles.protected"] = "защищена",
            ["roles.edit"] = "Изменить",
            ["roles.createRequest"] = "Создавать заявки",
            ["roles.viewHistory"] = "Просматривать историю",
            ["roles.manageEntry"] = "Вход / Выход (КПП)",
            ["roles.systemNameHint"] = "Латиница, без пробелов",

            // Тема
            ["theme.dark"] = "Тёмная тема",
            ["theme.light"] = "Светлая тема",
            ["theme.accent"] = "Цвет акцента",

            // Действия
            ["action.entry"] = "Въезд",
            ["action.exit"] = "Выезд",

            // Тестовые аккаунты
            ["login.testAccounts"] = "Тестовые аккаунты",

            // Лог
            ["log.title"] = "Лог действий",
            ["log.action"] = "Действие",
            ["log.user"] = "Пользователь",
            ["log.target"] = "Объект",
            ["log.time"] = "Время",

            // Время
            ["time.min"] = "мин",
            ["time.h"] = "ч",
        },
        ["ky"] = new()
        {
            ["app.title"] = "КПП — Конок көзөмөлү",
            ["app.subtitle"] = "Конок эсебин тутуу системасы",
            ["nav.history"] = "Тарых",
            ["nav.register"] = "Жаңы арыз",
            ["nav.users"] = "Колдонуучулар",
            ["nav.roles"] = "Ролдор жана укуктар",
            ["nav.menu"] = "Меню",
            ["nav.admin"] = "Администрация",
            ["nav.stats"] = "Статистика",
            ["btn.login"] = "Кирүү",
            ["btn.logout"] = "Чыгуу",
            ["btn.refresh"] = "Жаңылоо",
            ["btn.create"] = "Түзүү",
            ["btn.save"] = "Сактоо",
            ["btn.cancel"] = "Жокко чыгаруу",
            ["btn.delete"] = "Өчүрүү",
            ["btn.close"] = "Жабуу",
            ["lbl.login"] = "Логин",
            ["lbl.password"] = "Сырсөз",
            ["lbl.search"] = "Аты-жөнү, паспорт, авто номери...",
            ["lbl.username"] = "Логин",
            ["lbl.displayName"] = "Көрүнүүчү аты",
            ["lbl.role"] = "Роль",
            ["dashboard.title"] = "Башкаруу панели",
            ["dashboard.subtitle"] = "КПП ишмердүүлүгүнүн көзү",
            ["dashboard.inside"] = "Ичинде",
            ["dashboard.today"] = "Бүгүн",
            ["dashboard.week"] = "Жумада",
            ["dashboard.pending"] = "Күтүүдө",
            ["dashboard.exited"] = "Бүгүн чыгышкан",
            ["dashboard.total"] = "Жалпы",
            ["dashboard.recent"] = "Акыркы аракеттер",
            ["dashboard.waiting"] = "Күтүүдөгү коноктор",
            ["dashboard.visits"] = "Жумадагы коноктор",
            ["dashboard.peaks"] = "Пик сааттар",
            ["dashboard.noActions"] = "Азырынча аракет жок",
            ["dashboard.noPending"] = "Күтүүдөгү коноктор жок",
            ["dashboard.hour"] = "с",
            ["history.title"] = "Конок тарыхы",
            ["history.subtitle"] = "Коноктордун кириш-чыгыш эсеби",
            ["theme.dark"] = "Караңгы тема",
            ["theme.light"] = "Жарык тема",
            ["theme.accent"] = "Акцент түсү",
            ["login.testAccounts"] = "Сынак аккаунттары",
            ["time.min"] = "мүн",
            ["time.h"] = "с",
        },
        ["en"] = new()
        {
            ["app.title"] = "KPP — Guest Control",
            ["app.subtitle"] = "Visitor tracking system",
            ["nav.history"] = "History",
            ["nav.register"] = "New Request",
            ["nav.users"] = "Users",
            ["nav.roles"] = "Roles & Permissions",
            ["nav.menu"] = "Menu",
            ["nav.admin"] = "Administration",
            ["nav.stats"] = "Statistics",
            ["btn.login"] = "Login",
            ["btn.logout"] = "Logout",
            ["btn.refresh"] = "Refresh",
            ["btn.create"] = "Create",
            ["btn.save"] = "Save",
            ["btn.cancel"] = "Cancel",
            ["btn.delete"] = "Delete",
            ["btn.close"] = "Close",
            ["lbl.login"] = "Login",
            ["lbl.password"] = "Password",
            ["lbl.search"] = "Name, passport, license plate...",
            ["lbl.username"] = "Username",
            ["lbl.displayName"] = "Display Name",
            ["lbl.role"] = "Role",
            ["dashboard.title"] = "Dashboard",
            ["dashboard.subtitle"] = "KPP Activity Overview",
            ["dashboard.inside"] = "Inside",
            ["dashboard.today"] = "Today",
            ["dashboard.week"] = "This Week",
            ["dashboard.pending"] = "Pending",
            ["dashboard.exited"] = "Exited Today",
            ["dashboard.total"] = "Total",
            ["dashboard.recent"] = "Recent Activity",
            ["dashboard.waiting"] = "Pending Guests",
            ["dashboard.visits"] = "Weekly Visits",
            ["dashboard.peaks"] = "Peak Hours",
            ["dashboard.noActions"] = "No actions yet",
            ["dashboard.noPending"] = "No pending guests",
            ["dashboard.hour"] = "h",
            ["history.title"] = "Visit History",
            ["history.subtitle"] = "Guest entry & exit tracking",
            ["theme.dark"] = "Dark Theme",
            ["theme.light"] = "Light Theme",
            ["theme.accent"] = "Accent Color",
            ["login.testAccounts"] = "Test Accounts",
            ["time.min"] = "min",
            ["time.h"] = "h",
        }
    };

    public LocalizationService(IJSRuntime js) => _js = js;

    public async Task InitializeAsync()
    {
        try
        {
            var saved = await _js.InvokeAsync<string?>("localStorage.getItem", "kpp-lang");
            if (!string.IsNullOrWhiteSpace(saved) && Translations.ContainsKey(saved))
                CurrentLang = saved;
        }
        catch { }
        OnChange?.Invoke();
    }

    public async Task SetLangAsync(string lang)
    {
        if (!Translations.ContainsKey(lang)) return;
        CurrentLang = lang;
        try { await _js.InvokeVoidAsync("localStorage.setItem", "kpp-lang", lang); }
        catch { }
        OnChange?.Invoke();
    }

    public string this[string key]
    {
        get
        {
            if (Translations.TryGetValue(CurrentLang, out var dict) && dict.TryGetValue(key, out var val))
                return val;
            // Fallback to Russian
            if (Translations.TryGetValue("ru", out var ruDict) && ruDict.TryGetValue(key, out var ruVal))
                return ruVal;
            return key;
        }
    }

    public string T(string key) => this[key];
}
