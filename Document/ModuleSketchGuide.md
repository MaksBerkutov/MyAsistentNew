![LOGO](https://github.com/MaksBerkutov/MyAsistentNew/blob/master/ImageGit/Logo/Logo_Blue.svg)

# Інструкція до написання прошивки до модулів

## Початок

Для початку потрібно завантажити бібліотеку розробки з репозиторію проекту. Далі потрібно підключити бібліотеку до вашої скетч-програми за допомогою такого коду: `#include <Assistent.h>`.

Створіть екземпляр класу `AssistenWiFi` за допомогою наступного рядка коду: `AssistenWiFi assistent;`.

## Параметри функції `Begin`

Функція `Begin` має 14 параметрів:

1. Унікальне ім'я модуля.
2. Масив команд, які не повертають значення.
3. Масив обробників команд, які не повертають значення.
4. Розмір масивів команд, які не повертають значення.
5. Масив команд, які повертають значення.
6. Масив обробників команд, які повертають значення.
7. Розмір масивів команд, які повертають значення.
8. Розмір масивів команд, які повертають значення.
9. Назва мережі.
10. Пароль від мережі.
11. IP-адреса сервера.
12. Порт сервера.
13. Швидкість ком-порту.
14. Користувацький обробник.

## Використання сервера

Першим кроком є налаштування та запуск сервера для визначення IP-адреси та порту сервера. Далі ви можете розробити функціонал для кожного з модулів. Після цього ви можете перейти до написання прошивок для модулів. Попередні кроки були описані вище.

## Приклад створення та запуску екземпляру класу

```cpp
String NameBoard = "Arduino1";
AssistenWiFi assistent;

void setup() {
  pinMode(PinLed, OUTPUT);
  pinMode(ReleyPin1, OUTPUT);
  pinMode(ReleyPin2, OUTPUT);

  assistent.Begin(NameBoard, CMD, HCmd, 6, CMDRec, HCmdRec, 1, (char*)"SSID", (char*)"PASWORLD", (char*)"192.168.2.102", 11000);
}

void loop() {
  assistent.Reader();
}
```
## Як ввімкнути функцію OTA

Все дуже просто, вам потрібно на початку скетча прописати такий рядок коду `#define OTA`.

## Приклади обробників команд, які не повертають значення

```cpp
void ON() {
  State = true;
  digitalWrite(PinLed, LOW);
}

void OFF() {
  State = false;
  digitalWrite(PinLed, HIGH);
}

void ON_Rele1() {
  Rele1 = true;
  digitalWrite(ReleyPin1, HIGH);
}

void OFF_Rele1() {
  Rele1 = false;
  digitalWrite(ReleyPin1, LOW);
}

void ON_Rele2() {
  Rele2 = true;
  digitalWrite(ReleyPin2, HIGH);
}

void OFF_Rele2() {
  Rele2 = false;
  digitalWrite(ReleyPin2, LOW);
}
```
```cpp
HandlerCMD HCmd[] = {
  ON,
  OFF,
  ON_Rele1,
  OFF_Rele1,
  ON_Rele2,
  OFF_Rele2
};

String CMD[] = {
  "ON",
  "OFF",
  "ON_Rele1",
  "OFF_Rele1",
  "ON_Rele2",
  "OFF_Rele2"
};
```
## Приклад обробників команд, які повертають значення
```cpp
String CMDRec[] = {
  "GetStates"
};

String ReleToString(bool value) {
  if (value) {
    return "open";
  } else {
    return "close";
  }
}

AssistentVariable GetState() {
  return AssistentVariable(new String[3] { "State", "Rele1", "Rele2" },
 new String[3] { ReleToString(State), ReleToString(Rele1), ReleToString(Rele2) }, 3);
}

HandlerCMDRec HCmdRec[] = {
  GetState
};
```
