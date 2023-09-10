# CMS-MVC-.NET

INSTALACIJA PROJEKTA

1. Potrebno je nakon download-a projekta razdvojiti *.sln datoteku (pošto je naknadno dodana) od ostalih foldera i datoteka tako što u folder CMSProductSystem treba premjestiti sav sadržaj osim *.sln datoteke da izgleda na sljedeći način:
    CMSProductSystem (sa svim prethodno spomenutim sadržajem)
    .editorConfig
   CMSProductSystem.sln
3. Potrebno je iz foldera App_data locirati dvije datoteke mdf i ldf, te ih kopirati u putanju C:\Users\user_xx.
4. Korišten je MS SQL ali lokalna baza podataka.
5. Treba napraviti celan code-a i rebuild, te pokrenuti projekt.
6. Inicijalni login je: User: admin@admin.com Pass: Admin#123
Svaki pasword od bilo kojeg user-a je sljedeći: npr. bero@mail.com, pass je : Bero#123
7. Za autorizaciju je korištena opcija Individual User Accounts te su same metode za implementaicju prijave, regisstracije i odjave korisnika dorađene.
8. Proširen je Identity Managment sa dodatnim atributima: Adresa, GRad, Aktivan, FirstName, LastName, Telefeon...
10. Ako inicijalni login prođe, aplikacija se može koristiti.
11. Za provjeru rada API-ja (prikaz u JSON formatu), koristiti sljedeći primjer: https://localhost:7223/api/proizvodi/5 gdje je id proizvoda npr. 5
