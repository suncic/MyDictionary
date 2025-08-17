IZMENE I UNAPREDJENJA
-----------------------
- Dodato upravljanje faktorom zasićenosti (load factor) – za bolje performanse pri radu sa velikim duzinama lanaca kolizije.
- Optimizacija postojećih metoda i ispravka Hash metode (kontrola negativnih vrednosti).
- Proširen interfejs IMySimpleDictionary<TKey, TValue> – sada nasleđuje IEnumerable<KeyValuePair<TKey, TValue>> radi podrške foreach petljama i LINQ-u.
- Dodate metode koje imitiraju ponašanje ugrađenog Dictionary<TKey, TValue> u .NET-u:
    * void CopyTo(KeyValuePair<TKey, TValue>[] array, int index)
    * void Add(KeyValuePair<TKey, TValue> item)
    * bool Contains(KeyValuePair<TKey, TValue> item)
    * bool Remove(KeyValuePair<TKey, TValue> item)
    * bool TryAdd(TKey key, TValue value)
    * bool TryGetValue(TKey key, out TValue value)
- Generalna poboljšanja koda radi bolje čitljivosti i robusnosti.

U ovoj implementaciji odlucila sam se protiv zatvorenog hesiranja, iako ge .NET Dictionary<K, V> koristi za hesiranje, iz sledecih razloga:

1. Bolje performanse pri velikom broju kolizija
    * kod zatvorenog hesiranja, kad tabela postane popunjena (fc > 0.7), probiranje postaje sve sporije
    * u otvorenom heiranju, svi elementi sa istim hesom se cuvaju u jednoj listi => performanse ostaju stabilnije i kad je tabela puna

2. Izbor hes funkcije
    * kod zatvorenog hesiranja, losa hes funkcija direktno utice na broj kolizija i dužinu probe
    * kod otvorenog hesiranje elementi sa istim indeksom se grupisu u listu (ne traže se nova slobodna mesta kroz tabelu)

3. Efikasnije brisanje
    *  kod zatvorenog hesiranja moramo da koristimo oznake, da bi znali da li smemo da obrisemo element
    * kod zatvorenog imamo prevezivanje pokazivaca sa jednog elementa na drugi

4. Velicina tabele
    * u zatvorenom hesiranju, moramo da pravimo novu tabelu (vecu), i svi elementi moraju ponovo da se probiraju
    * kod otvorenog, jedna lista se 'premest' na drugo polje niza sa vrednosti hes funkcija

5. Rukovanje brojem elemenata
    * zatvoreno hesiranje, zahteva veliki niz sa slobodnim mestima
    * otvoreno hesiranje, omogucava da tabela sadrzi vise elemenata nego sto ona ima polja

Iako zatvoreno hesiranje nudi bolje performanse u slučajevima sa dobro raspodeljenim kljucevima i niskim stepenom popunjenosti tabele, otvoreno hesiranje sam izabrala zbog stabilnijeg ponasanja u uslovima sa vecim brojem kolizija, jednostavnijeg brisanja i manje zavisnosti od kvaliteta hes funkcije.
