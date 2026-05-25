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
            ["register.ocrScanning"] = "Распознавание паспорта...",
            ["register.ocrDone"] = "Автозаполнено",
            ["register.ocrFieldsOf4"] = "из 4 полей",
            ["register.ocrVerify"] = "Проверьте данные перед отправкой",
            ["register.ocrError"] = "Не удалось распознать паспорт. Заполните вручную.",

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
            ["users.newUser"] = "Новый пользователь",
            ["users.fillAll"] = "Заполните все поля",
            ["users.loginTaken"] = "Логин уже занят",
            ["users.userCreated"] = "Пользователь создан",
            ["users.passwordChanged"] = "Пароль изменён",
            ["users.statusChanged"] = "Статус изменён",
            ["users.deleted"] = "Удалён",

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
            ["roles.newRole"] = "Новая роль",
            ["roles.fillFields"] = "Заполните поля",
            ["roles.permissions"] = "Права доступа",
            ["roles.roleCreated"] = "Роль создана",
            ["roles.roleUpdated"] = "Роль обновлена",
            ["roles.roleDeleted"] = "Роль удалена",

            // Тема
            ["theme.dark"] = "Тёмная тема",
            ["theme.light"] = "Светлая тема",
            ["theme.accent"] = "Цвет акцента",

            // Действия
            ["action.entry"] = "Въезд",
            ["action.exit"] = "Выезд",

            // Тестовые аккаунты
            ["login.testAccounts"] = "Тестовые аккаунты",
            ["login.enterCredentials"] = "Введите логин и пароль",
            ["login.wrongCredentials"] = "Неверный логин или пароль",
            ["login.accountBlocked"] = "Аккаунт заблокирован",
            ["login.loggingIn"] = "Вход...",

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
            // Общие
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

            // Дашборд
            ["dashboard.title"] = "Башкаруу панели",
            ["dashboard.subtitle"] = "КПП ишмердүүлүгүнүн көзөмөлү",
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

            // История
            ["history.title"] = "Конок тарыхы",
            ["history.subtitle"] = "Коноктордун кириш-чыгыш эсеби",
            ["history.guest"] = "Конок",
            ["history.purpose"] = "Максат / Кабыл алгыш",
            ["history.car"] = "Авто",
            ["history.entry"] = "Кириш",
            ["history.exit"] = "Чыгуу",
            ["history.time"] = "Убакыт",
            ["history.status"] = "Статус",
            ["history.docs"] = "Документтер",
            ["history.actions"] = "Аракеттер",
            ["history.all"] = "Баары",
            ["history.pending"] = "Күтүүдө",
            ["history.inside"] = "Ичинде",
            ["history.exited"] = "Чыккан",
            ["history.found"] = "Табылды:",
            ["history.today"] = "Бүгүн",
            ["history.week"] = "7 күн",
            ["history.reset"] = "Тазалоо",
            ["history.from"] = "Баштап:",
            ["history.to"] = "Чейин:",
            ["history.noRecords"] = "Фильтрлер боюнча жазылгалар жок",
            ["history.loading"] = "Жүктөлүүдө...",

            // Регистрация
            ["register.title.admin"] = "Жаңы арыз",
            ["register.title.kpp"] = "Конокту каттоо",
            ["register.title.user"] = "Арыз түзүү",
            ["register.purpose"] = "Келүү максаты",
            ["register.host"] = "Кабыл алгыш жак",
            ["register.plannedDate"] = "Пландык күн",
            ["register.transport"] = "Транспорт",
            ["register.optional"] = "милдеттүү эмес",
            ["register.carBrand"] = "Марка",
            ["register.carPlate"] = "Мамлекеттик номер",
            ["register.guestData"] = "Конок маалыматы",
            ["register.oneGuest"] = "Бир конок",
            ["register.group"] = "Топ",
            ["register.fullName"] = "Аты-жөнү",
            ["register.dob"] = "Туулган күнү",
            ["register.passport"] = "Серия жана номер",
            ["register.nationality"] = "Жарандык",
            ["register.passportScan"] = "Паспорт сканы",
            ["register.permitDoc"] = "Өткөрүү документи",
            ["register.loaded"] = "Жүктөлдү",
            ["register.addGuest"] = "Конок кошуу",
            ["register.guest"] = "Конок",
            ["register.create"] = "Арыз түзүү",
            ["register.submitKpp"] = "Каттоо",
            ["register.submitUser"] = "Арыз жөнөтүү",
            ["register.saving"] = "Сактоо...",
            ["register.registered"] = "Конок катталды!",
            ["register.created"] = "Арыз түзүлдү!",
            ["register.guestsRegistered"] = "конок ийгиликтүү катталды",
            ["register.guestsAdded"] = "конок ийгиликтүү кошулду",
            ["register.guestRegistered"] = "Конок ийгиликтүү катталды",
            ["register.guestAdded"] = "Конок күтүү тизмесине кошулду",
            ["register.toHistory"] = "Тарыхка",
            ["register.newRequest"] = "Жаңы арыз",
            ["register.banner.admin"] = "Арыз «Күтүүдө» статусу менен түзүлөт.",
            ["register.banner.kpp"] = "Конок «Ичинде» статусу менен катталат.",
            ["register.banner.user"] = "Сиздин арыз КПП-га жөнөтүлөт.",
            ["register.ocrScanning"] = "Паспорт таанытылууда...",
            ["register.ocrDone"] = "Автотолтурулду",
            ["register.ocrFieldsOf4"] = "4 талаадан",
            ["register.ocrVerify"] = "Жиберүүдөн мурун маалыматтарды текшериңиз",
            ["register.ocrError"] = "Паспорт таанылган жок. Кол менен толтуруңуз.",

            // Пользователи
            ["users.title"] = "Колдонуучулар",
            ["users.createUser"] = "Колдонуучу түзүү",
            ["users.user"] = "Колдонуучу",
            ["users.status"] = "Статус",
            ["users.created"] = "Түзүлдү",
            ["users.actions"] = "Аракеттер",
            ["users.active"] = "Активдүү",
            ["users.blocked"] = "Блокталды",
            ["users.block"] = "Блоктоо",
            ["users.unblock"] = "Блоктонуу",
            ["users.password"] = "Сырсөз",
            ["users.newPassword"] = "Жаңы сырсөз",
            ["users.chooseRole"] = "— танданыз —",
            ["users.newUser"] = "Жаңы колдонуучу",
            ["users.fillAll"] = "Бардык талааларды толтуруңуз",
            ["users.loginTaken"] = "Логин эле алынган",
            ["users.userCreated"] = "Колдонуучу түзүлдү",
            ["users.passwordChanged"] = "Сырсөз өзгөрдү",
            ["users.statusChanged"] = "Статус өзгөрдү",
            ["users.deleted"] = "Өчүрүлдү",

            // Роли
            ["roles.title"] = "Ролдор жана укуктар",
            ["roles.createRole"] = "Роль түзүү",
            ["roles.name"] = "Аты",
            ["roles.systemName"] = "Системдик аты",
            ["roles.requests"] = "Арыздар",
            ["roles.history"] = "Тарых",
            ["roles.entryExit"] = "Кириш/Чыгуу",
            ["roles.actions"] = "Аракеттер",
            ["roles.system"] = "системдик",
            ["roles.protected"] = "корголгон",
            ["roles.edit"] = "Өзгөртүү",
            ["roles.createRequest"] = "Арыз түзүү",
            ["roles.viewHistory"] = "Тарыхты көрүү",
            ["roles.manageEntry"] = "Кириш / Чыгуу (КПП)",
            ["roles.systemNameHint"] = "Латынча, боштуксыз",
            ["roles.newRole"] = "Жаңы роль",
            ["roles.fillFields"] = "Талааларды толтуруңуз",
            ["roles.permissions"] = "Укуктар",
            ["roles.roleCreated"] = "Роль түзүлдү",
            ["roles.roleUpdated"] = "Роль жаңыланды",
            ["roles.roleDeleted"] = "Роль өчүрүлдү",

            // Тема
            ["theme.dark"] = "Караңгы тема",
            ["theme.light"] = "Жарык тема",
            ["theme.accent"] = "Акцент түсү",

            // Действия
            ["action.entry"] = "Кириш",
            ["action.exit"] = "Чыгуу",

            // Тестовые аккаунты
            ["login.testAccounts"] = "Сынак аккаунттары",
            ["login.enterCredentials"] = "Логин жана сырсөзду киргизиңиз",
            ["login.wrongCredentials"] = "Туура эмес логин же сырсөз",
            ["login.accountBlocked"] = "Аккаунт блокталды",
            ["login.loggingIn"] = "Кирүү...",

            // Лог
            ["log.title"] = "Аракеттер логу",
            ["log.action"] = "Аракет",
            ["log.user"] = "Колдонуучу",
            ["log.target"] = "Объект",
            ["log.time"] = "Убакыт",

            // Время
            ["time.min"] = "мүн",
            ["time.h"] = "с",
        },
        ["en"] = new()
        {
            // General
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

            // Dashboard
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

            // History
            ["history.title"] = "Visit History",
            ["history.subtitle"] = "Guest entry & exit tracking",
            ["history.guest"] = "Guest",
            ["history.purpose"] = "Purpose / Host",
            ["history.car"] = "Vehicle",
            ["history.entry"] = "Entry",
            ["history.exit"] = "Exit",
            ["history.time"] = "Time",
            ["history.status"] = "Status",
            ["history.docs"] = "Documents",
            ["history.actions"] = "Actions",
            ["history.all"] = "All",
            ["history.pending"] = "Pending",
            ["history.inside"] = "Inside",
            ["history.exited"] = "Exited",
            ["history.found"] = "Found:",
            ["history.today"] = "Today",
            ["history.week"] = "7 days",
            ["history.reset"] = "Reset",
            ["history.from"] = "From:",
            ["history.to"] = "To:",
            ["history.noRecords"] = "No records match the given filters",
            ["history.loading"] = "Loading...",

            // Registration
            ["register.title.admin"] = "New Request",
            ["register.title.kpp"] = "Register Guest",
            ["register.title.user"] = "Create Request",
            ["register.purpose"] = "Visit Purpose",
            ["register.host"] = "Host Person",
            ["register.plannedDate"] = "Planned Date",
            ["register.transport"] = "Transport",
            ["register.optional"] = "optional",
            ["register.carBrand"] = "Brand",
            ["register.carPlate"] = "License Plate",
            ["register.guestData"] = "Guest Information",
            ["register.oneGuest"] = "Single Guest",
            ["register.group"] = "Group",
            ["register.fullName"] = "Full Name",
            ["register.dob"] = "Date of Birth",
            ["register.passport"] = "Series & Number",
            ["register.nationality"] = "Nationality",
            ["register.passportScan"] = "Passport Scan",
            ["register.permitDoc"] = "Permit Document",
            ["register.loaded"] = "Loaded",
            ["register.addGuest"] = "Add Guest",
            ["register.guest"] = "Guest",
            ["register.create"] = "Create Request",
            ["register.submitKpp"] = "Register",
            ["register.submitUser"] = "Submit Request",
            ["register.saving"] = "Saving...",
            ["register.registered"] = "Guest registered!",
            ["register.created"] = "Request created!",
            ["register.guestsRegistered"] = "guests successfully registered",
            ["register.guestsAdded"] = "guests successfully added",
            ["register.guestRegistered"] = "Guest successfully registered",
            ["register.guestAdded"] = "Guest successfully added to the waiting list",
            ["register.toHistory"] = "To History",
            ["register.newRequest"] = "New Request",
            ["register.banner.admin"] = "Request will be created with «Pending» status.",
            ["register.banner.kpp"] = "Guest will be registered with «Inside» status.",
            ["register.banner.user"] = "Your request will be forwarded to the checkpoint.",
            ["register.ocrScanning"] = "Scanning passport...",
            ["register.ocrDone"] = "Auto-filled",
            ["register.ocrFieldsOf4"] = "of 4 fields",
            ["register.ocrVerify"] = "Verify the data before submitting",
            ["register.ocrError"] = "Failed to recognize passport. Fill manually.",

            // Users
            ["users.title"] = "Users",
            ["users.createUser"] = "Create User",
            ["users.user"] = "User",
            ["users.status"] = "Status",
            ["users.created"] = "Created",
            ["users.actions"] = "Actions",
            ["users.active"] = "Active",
            ["users.blocked"] = "Blocked",
            ["users.block"] = "Block",
            ["users.unblock"] = "Unblock",
            ["users.password"] = "Password",
            ["users.newPassword"] = "New Password",
            ["users.chooseRole"] = "— select —",
            ["users.newUser"] = "New User",
            ["users.fillAll"] = "Fill all fields",
            ["users.loginTaken"] = "Login already taken",
            ["users.userCreated"] = "User created",
            ["users.passwordChanged"] = "Password changed",
            ["users.statusChanged"] = "Status changed",
            ["users.deleted"] = "Deleted",

            // Roles
            ["roles.title"] = "Roles & Permissions",
            ["roles.createRole"] = "Create Role",
            ["roles.name"] = "Name",
            ["roles.systemName"] = "System Name",
            ["roles.requests"] = "Requests",
            ["roles.history"] = "History",
            ["roles.entryExit"] = "Entry/Exit",
            ["roles.actions"] = "Actions",
            ["roles.system"] = "system",
            ["roles.protected"] = "protected",
            ["roles.edit"] = "Edit",
            ["roles.createRequest"] = "Create Requests",
            ["roles.viewHistory"] = "View History",
            ["roles.manageEntry"] = "Entry / Exit (Checkpoint)",
            ["roles.systemNameHint"] = "Latin, no spaces",
            ["roles.newRole"] = "New Role",
            ["roles.fillFields"] = "Fill fields",
            ["roles.permissions"] = "Permissions",
            ["roles.roleCreated"] = "Role created",
            ["roles.roleUpdated"] = "Role updated",
            ["roles.roleDeleted"] = "Role deleted",

            // Theme
            ["theme.dark"] = "Dark Theme",
            ["theme.light"] = "Light Theme",
            ["theme.accent"] = "Accent Color",

            // Actions
            ["action.entry"] = "Entry",
            ["action.exit"] = "Exit",

            // Test Accounts
            ["login.testAccounts"] = "Test Accounts",
            ["login.enterCredentials"] = "Enter login and password",
            ["login.wrongCredentials"] = "Invalid login or password",
            ["login.accountBlocked"] = "Account is blocked",
            ["login.loggingIn"] = "Logging in...",

            // Log
            ["log.title"] = "Action Log",
            ["log.action"] = "Action",
            ["log.user"] = "User",
            ["log.target"] = "Target",
            ["log.time"] = "Time",

            // Time
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
