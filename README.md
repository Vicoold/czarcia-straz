# Czarcia Straż (Slavic Witch TD)

> **Mobile Tower Defense / Action RTS Hybrid**  
> Inspiracje: *Thronefall*, *Twierdza (Stronghold)*, *Kingdom Rush: Alliance*, *Cult of the Lamb*, słowiański folklor Podlasia.

---

## 🎮 O Projekcie

Projekt tworzony na 48h Hackathon. Gracz wciela się w słowiańską Wiedźmę zakładającą osadę wokół dawnej drewnianej cerkwi przerobionej na okultystyczne sanktuarium. 
* **Cykl Dnia:** Swobodne planowanie fundamentów budynków na siatce (Grid 1.5m), dobieganie Wiedźmą i fizyczne napełnianie placu budowy monetami/sianem.
* **Cykl Nocy:** Walka na pierwszej linii, obrona drewnianych murów z lampionami rytualnymi, loadout 3 potężnych czarów, sojuszniczy kultyści blokujący wąskie gardła (chokepoints) i fale wrogów podzielone na 3 Akty.
* **Dynamiczna Ewolucja:** Teren mapy zmienia się po wyznaczonych falach (np. zalanie rzeki, otwarcie bagien od wschodu, przebudzenie pradawnej puszczy).

---

## 📐 Dokumentacja Techniczna & GDD
Pełna specyfikacja architektoniczna, formuły ekonomiczne, interfejsy C# oraz podział zadań dla agentów znajdują się w:
👉 [`docs/superpowers/specs/2026-10-25-slavic-witch-td-design.md`](docs/superpowers/specs/2026-10-25-slavic-witch-td-design.md)

---

## 🎨 Wizualia i Assety
* **Koncepcja wizualna:** `Assets/_Project/Art/ConceptArt/GameplayFrame_Concept.jpg`
* **Modele 3D (Podlasie/Slavic):** `Assets/_Project/Art/Models/Environment/Cottage/`
  * `Cottage.fbx` (Chałupa słowiańska – generator populacji)
  * `Podlasie_Barn_Puchly_62.fbx` (Koszary kultystów)
  * `Podlasie_Picket_Fence.fbx` & `Podlasie_Balas_Fence_Bialowieza.fbx` (Palisady i ogrodzenia)
  * `AppleTree_Summer.fbx` (Drzewa i roślinność)

---

## 🛠️ Architektura i Pakiety Zadań dla Agentów
1. **Pakiet A:** `SlavicTD.Grid` – Siatka, fundamenty `BlueprintInstance`, autotiling murów `WallAutoTile`.
2. **Pakiet B:** `SlavicTD.Player` – Kontroler Wiedźmy, wirtualny joystick, system 3 czarów `SpellCaster`.
3. **Pakiet C:** `SlavicTD.Economy` – Siano, dochód o świcie, populacja z chałup, sekwencyjna animacja montażu.
4. **Pakiet D:** `SlavicTD.Waves` & `SlavicTD.Enemies` – Fale wrogów (3 Akty), AI niszczenia murów na NavMeshu.
5. **Pakiet E:** `SlavicTD.Core` & `SlavicTD.UI` – Maszyna stanów `GameFlowManager`, dynamiczna mapa `MapEvolutionTrigger`, HUD dotykowy.
