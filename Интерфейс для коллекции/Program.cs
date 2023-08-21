using System;
using System.Collections;
using System.Linq.Expressions;

namespace app45_IDictionary
{
    public class SimpleDictionary : IDictionary
    { // Словарь (коллекция, массив элементов)
        private DictionaryEntry[] items;
        private int ItemsInUse = 0; // Поле
                                      // Конструктор, задающий определенное
                                      // количество элементов
        public SimpleDictionary(int numItems)
        {
            items = new DictionaryEntry[numItems]; // Присвоение
                                                   // массивов возможно
        }
        #region IDictionary Members // Специальная область,
        // позволяющая редактору среды
        // сворачивать, разворачивать ее содержимое
        // для удобства пользования (начало)
        public bool IsReadOnly
        { get { return false; } } // Свойство
                                  // Метод проверяет, содержится ли в экземпляре
                                  // заданный ключ
        public bool Contains(object key)
        {
            int index;
            return TryGetIndexOfKey(key, out index); // Метод
                                                     // проверяет, содержится ли в экземпляре
                                                     // заданный ключ
        }
        // Свойство показывает, имеет ли объект
        // фиксированный размер
        public bool IsFixedSize { get { return false; } }
        // Метод удаляет из объекта элемент с заданным ключом
        public void Remove(object key)
        {
            if (key == null)

                throw new ArgumentNullException("key"); // Ключ
                                                        // не найден
                                                        // Попытка найти ключ в массиве DictionaryEntry
            int index;
            if (TryGetIndexOfKey(key, out index))
            {
                // Если ключ найден, просмотреть все элементы
                Array.Copy(items, index + 1, items, index,
                ItemsInUse - index - 1);
                ItemsInUse--;
            }
            else
            { // Если ключ не найден – выход }
            }
        }

        private void Clear1()
        { ItemsInUse = 0; }

        private void Add1(object key, object value)
        {
            // Добавление новой пары "ключ — значение",
            // даже если такой ключ существует в массиве
            if (ItemsInUse == items.Length)
                throw new InvalidOperationException("Массив не может" +
                "содержать больше элементов");
            items[ItemsInUse++] = new DictionaryEntry(key,
            value);
        }
        public ICollection Keys
        {
            get
            { // Возвращает массив ключей:
                /* В поле ItemsInUse к этому моменту расчета
                находится количество элементов словаря*/

                Object[] keys = new Object[ItemsInUse];
                for (Int32 n = 0; n < ItemsInUse; n++)
                    keys[n] = items[n].Key;
                return keys;
            }
        }
        // Свойство, как и Keys: в нем отдельно формируются
        // значения словаря, т. е. словарь разбивается
        // на два отдельных массива — ключи и значения
        public ICollection Values
        {
            get
            { // Возвращает массив значений
                Object[] values = new Object[ItemsInUse];
                for (Int32 n = 0; n < ItemsInUse; n++)
                    values[n] = items[n].Value;
                return values;
            }
        }
        // Задание индексатора, чтобы можно было искать
        // элемент массива по его индексу: он участвует
        // в поиске по индексу неявно. Как только встречается
        // переменная с индексом, управление передается
        // индексатору для ее вычисления
        public object this[object key]
        {
            get
            { // Если такой ключ есть в массиве, выдать
              // соответствующее ему значение
                Int32 index;
                if (TryGetIndexOfKey(key, out index))
                { // Ключ найден, возврат соответствующего
                  // значения
                    return items[index].Value;
                }
                else
                { // Ключ не найден
                    return null;
                }
            }

            set
            { // Если заданный ключ имеется в массиве, изменить
              // соответствующее ему значение
                Int32 index;
                if (TryGetIndexOfKey(key, out index))
                { // Если ключ найден, изменить
                  // его значение
                    items[index].Value = value;
                }
                else
                { // Если ключ не найден, добавить в массив пару
                  // "ключ — значение"
                    Add(key, value);
                }
            }
        }
        private Boolean TryGetIndexOfKey(Object key,
        out Int32 index)
        {
            for (index = 0; index < ItemsInUse; index++)
            { // Если ключ найден, вернуть true (индекс также
              // возвращается через параметр метода)
                if (items[index].Key.Equals(key)) return true;
            }
            // Ключ не найден, возврат false (index должен быть
            // проигнорирован в вызывающем методе).
            return false;
        }
        private class SimpleDictionaryEnumerator :
        IDictionaryEnumerator
        {
            // Копирование пар из объекта SimpleDictionary
            DictionaryEntry[] items;
            Int32 index = -1;
            public SimpleDictionaryEnumerator(SimpleDictionary sd)
            {
                // Создание копии DictionaryEntry в простом массиве
                items = new DictionaryEntry[sd.Count];
                Array.Copy(sd.items, 0, items, 0, sd.Count);
            }

            // Возврат текущего элемента
            public object Current
            { get { ValidateIndex(); return items[index]; } }
            // Возврат текущего входа
            public DictionaryEntry Entry
            {
                get { return (DictionaryEntry)Current; }
            }
            // Возврат ключа текущего элемента
            public object Key
            { get { ValidateIndex(); return items[index].Key; } }
            // Возврат значения текущего элемента
            public object Value
            { get { ValidateIndex(); return items[index].Value; } }
            // Переход к следующему элементу
            public bool MoveNext()
            {
                if (index < items.Length - 1)
                { index++; return true; }
                return false;
            }
            // Проверка на действительность индекса перечислителя
            // и выдача исключения, если индекс находится
            // вне границ
            private void ValidateIndex()
            {
                if (index < 0 || index >= items.Length)
                    throw new InvalidOperationException("Перечислитель" +
                    " вне границ коллекции");
            }
            // Восстановление индекса для перезапуска
            // перечислителя
            public void Reset()
            { index = -1; }
        }

        public IDictionaryEnumerator GetEnumerator()
        { // Создание перечислителя
            return new SimpleDictionaryEnumerator(this);
        }
        #endregion // Специальная область, позволяющая
        // редактору среды сворачивать,
        // разворачивать ее содержимое для удобства
        // пользования (конец области)
        // Члены интерфейса ICollection: должны быть
        // обязательно определены в производном классе
        // (таковы правила C#), хотя могут и не участвовать
        // в расчетах
        #region ICollection Members
        public bool IsSynchronized
        { get { return false; } }
        public object SyncRoot
        { get { throw new NotImplementedException(); } }
        public int Count { get { return ItemsInUse; } }
        public void CopyTo(Array array, int index)
        { throw new NotImplementedException(); }
        #endregion
        #region IEnumerable Members // Члены интерфейса
        // "Перечислитель"
        IEnumerator IEnumerable.GetEnumerator()
        { // Создание перечислителя
            return ((IDictionary)this).GetEnumerator();
        }

        public void Add(object key, object value)
        {
            throw new NotImplementedException();
        }
        
        public void Clear()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
    public sealed class App
    {
        static void Main()
        {
            // Создание словаря, содержащего не более трех входов
            IDictionary d = new SimpleDictionary(3)
            {
                // Добавление трех человек с их возрастами к словарю.
                { "Иван", 40 },

                { "Петр", 34 },
                { "Андрей", 1 }
            };
            Console.WriteLine("Количество элементов в словаре = {0}",
            d.Count);
            Console.WriteLine("Содержит ли словарь 'Ивана'? {0}",
            d.Contains("Иван"));
            /* Здесь срабатывает индексатор: как только
            встречается переменная с индексом (в данном случае
            d["Иван"]), управление передается индексатору,
            который и вычисляет значение переменной с индексом.
            Индекс должен быть заданного типа (в нашем случае
            это строка символов)) */
            Console.WriteLine("Возраст Ивана: {0}", d["Иван"]);
            // Вывод каждого входного ключа и к нему значения
            foreach (DictionaryEntry de in d)
            {
                Console.WriteLine("{0}: Возраст равен {1} годам",
                de.Key, de.Value);
            }
            // Удаление одного существующего входа
            Console.WriteLine("Удаление входа с ключом 'Иван'");
            d.Remove("Иван");
            // Удаление несуществующего входа без выдачи
            // исключения.
            Console.WriteLine("Удаление несуществующего " +
            "входа с ключом 'Макс'");
            d.Remove("Макс");
            // Вывод имен (это ключи) людей в словаре.
            // Keys — свойство класса, в котором формируются
            // отдельно ключи словаря
            Console.WriteLine("Вывод ключей словаря");
            foreach (string s in d.Keys)
                Console.WriteLine(s);
            
        // Вывод возрастов (это значения) людей в словаре
Console.WriteLine("Вывод значений словаря");
            foreach (int age in d.Values)
                Console.WriteLine(age);
            Console.Read();
        }
    }
}