using DG.Tweening;
using GreatGames.CaseLib.DI;
using GreatGames.CaseLib.Key;
using GreatGames.CaseLib.Patterns;
using GreatGames.CaseLib.Signals;
using GreatGames.CaseLib.Slinky;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace GreatGames.CaseLib.Grid
{
    public class GridManager : FoundationSingleton<GridManager>, IFoundationSingleton, IInitializable
    {
        [Header("Grid Configurations")]
        [SerializeField] private GameObject _gridPrefab;
        [SerializeField] private SlinkyController _slinkyPrefab;

        [Header("Grid Offsets")]
        [SerializeField] private Vector3 _slinkyGridOffset = new Vector3(0, 0, 0);
        [SerializeField] private Vector3 _mergeGridOffset = new Vector3(0, -1, 0);

        private Transform _gridParent;
        private Transform _slinkyParent;

        private GridStructure _upperGrid;
        private GridStructure _lowerGrid;

        public BasicSignal OnGridUpdated { get; private set; }
        public bool Initialized { get; set; }

        private LevelConfigSO _levelData;

       private List<SlinkyController> _slinkies = new List<SlinkyController>();

        private void OnEnable()
        {
            OnGridUpdated = new BasicSignal();
        }

        public void InitializeGrids(LevelConfigSO levelData, Transform levelParent)
        {
            _levelData = levelData;

            if (_levelData == null)
            {
                return;
            }

            if (_gridParent == null)
            {
                _gridParent = new GameObject("Grids Parent").transform;
                _gridParent.SetParent(levelParent);
            }

            if (_slinkyParent == null)
            {
                _slinkyParent = new GameObject("Slinkies Parent").transform;
                _slinkyParent.SetParent(levelParent);
            }

            if (_upperGrid == null)
            {
                _upperGrid = new GridStructure(_levelData.UpperGridSize, _slinkyGridOffset, _gridPrefab, true);
            }

            if (_lowerGrid == null)
            {
                _lowerGrid = new GridStructure(_levelData.LowerGridSize, _mergeGridOffset, _gridPrefab, false);
            }

            _upperGrid.InitializeGrid(_levelData.UpperGridSize, _gridParent);
            _lowerGrid.InitializeGrid(_levelData.LowerGridSize, _gridParent);

            OnGridUpdated?.Emit();

            SpawnSlinkies();
        }

        private void SpawnSlinkies()
        {
            if (_levelData == null || _slinkyPrefab == null)
            {
                return;
            }

            foreach (var slinkyData in _levelData.Slinkies)
            {
                GridStructure startGrid = _upperGrid;
                GridStructure endGrid = _upperGrid;

                GameKey startKey = new GameKey($"U_{slinkyData.StartSlot % _levelData.UpperGridSize.x},{slinkyData.StartSlot / _levelData.UpperGridSize.x}");
                GameKey endKey = new GameKey($"U_{slinkyData.EndSlot % _levelData.UpperGridSize.x},{slinkyData.EndSlot / _levelData.UpperGridSize.x}");

                Vector3 startPos = startGrid.GetWorldPosition(startKey);
                Vector3 endPos = endGrid.GetWorldPosition(endKey);

                GameObject startSlotObject = startGrid.GetSlotObject(startKey);
                GameObject endSlotObject = endGrid.GetSlotObject(endKey);

                GameObject slinkyContainer = new GameObject($"Slinky_{startKey.ValueAsString}");
                slinkyContainer.transform.SetParent(_slinkyParent);
                slinkyContainer.transform.position = startPos;

                SlinkyController slinky = Instantiate(_slinkyPrefab, startPos, Quaternion.identity);
                slinky.transform.SetParent(slinkyContainer.transform);
                slinky.Initialize(startPos, endPos, this, slinkyData.Color, startSlotObject, endSlotObject, slinkyContainer.transform);
                Debug.Log("Slinkies" + slinkyData.Color);
                startGrid.SetSlotOccupied(startKey, slinky);
                endGrid.SetSlotOccupied(endKey, slinky);
            }
        }

        public bool TryPlaceSlinky(GameKey slotKey, SlinkyController movingSlinky, bool isUpperGrid)
        {
            GridStructure targetGrid = isUpperGrid ? _upperGrid : _lowerGrid;

            if (targetGrid.TryGetContainer(slotKey, out GridDataContainer slot) && !slot.IsOccupied)
            {
                Debug.Log($"🧪 TryPlaceSlinky → {slotKey.ValueAsString} | IsOccupied = {slot.IsOccupied}");

                targetGrid.SetSlotOccupied(slotKey, movingSlinky);

                if (movingSlinky != null)
                {
                    movingSlinky.OccupiedGridKeys.Clear();
                    movingSlinky.OccupiedGridKeys.Add(slotKey);
                }
                else
                {
                    Debug.LogWarning($"[ERROR] {slotKey.ValueAsString} for slinky notFound!");
                }

                OnGridUpdated?.Emit();
             //  DebugLowerGridColors();
                return true;
            }
            return false;
        }


        public void RegisterSlinky(SlinkyController slinky)
        {
            _slinkies.Add(slinky);
        }
        public void RemoveSlinky(SlinkyController slinky)
        {
            if (slinky == null)
            {
                Debug.LogWarning("⚠️ RemoveSlinky: slinky is null!");
                return;
            }

            if (slinky.OccupiedGridKeys.Count > 0)
            {
                foreach (var key in slinky.OccupiedGridKeys)
                {
                    Debug.Log($"🧹 RemoveSlinky > Slot temizleniyor: {key.ValueAsString}");
                    if (_lowerGrid.TryGetContainer(key, out var slot))
                    {
                        slot.RemoveSlinky(); // bu SetSlinky(null) + IsOccupied false + signal
                    }
                }
            }

            slinky.SlotIndex = null;
            _slinkies.Remove(slinky);
        }

        public void RemoveSlinkyAt(GameKey key)
        {
            GridDataContainer container = null;

            if (key.IsLower())
                _lowerGrid.TryGetContainer(key, out container);
            else
                _upperGrid.TryGetContainer(key, out container);

            if (container != null && container.HasSlinky)
            {
                var slinky = container.Slinky; // ÖNCE al
                Debug.Log($"🧹 Slot temizleniyor: {key.ValueAsString}");
                container.Slinky.OccupiedGridKeys.Clear();
                if (key.IsLower())
                    _lowerGrid.ClearSlot(key);
                else
                    _upperGrid.ClearSlot(key);

                // ⛔ Bu kontrol Clear'dan önce yapılmalı
                if (slinky != null)
                {
                    RemoveSlinky(slinky); // Tüm referansları kaldır
                }
            }
        }
        public void LogLowerGridSlotStates()
        {
            Debug.Log("📊 Lower Grid Slot Durumları:");

            foreach (var kvp in _lowerGrid.GetAllSlots().OrderBy(k => k.Key.ToVector2Int().x))
            {
                string status = kvp.Value.IsOccupied ? "IsOccupied" : "Empty";
                Debug.Log($"{kvp.Key.ValueAsString} : {status}");
            }
        }

        public bool IsSlotEmpty(GameKey key, bool isUpperGrid)
        {
            return isUpperGrid ? _upperGrid.IsSlotEmpty(key) : _lowerGrid.IsSlotEmpty(key);
        }

        public Vector3 GetSlotPosition(GameKey key, bool isUpperGrid)
        {
            return isUpperGrid ? _upperGrid.GetWorldPosition(key) : _lowerGrid.GetWorldPosition(key);
        }
        public GameKey GetFirstEmptySlot(bool isUpperGrid)
        {
            var grid = isUpperGrid ? _upperGrid : _lowerGrid;

            return grid.GetAllSlots()
                .Where(kvp => !kvp.Value.IsOccupied)
                .OrderByDescending(kvp => kvp.Key.ToVector2Int().x)
                .Select(kvp => kvp.Key)
                .FirstOrDefault();
        }
        public List<GameKey> GetEmptySlots(bool isUpperGrid)
        {
            var grid = isUpperGrid ? _upperGrid : _lowerGrid;
            return grid.GetAllSlots()
                .Where(kvp => !kvp.Value.IsOccupied) 
                .OrderByDescending(kvp => kvp.Key.ToVector2Int().x) 
                .Select(kvp => kvp.Key) 
                .ToList(); 
        }
        public bool IsThereLongerSlinkyBlocking(SlinkyController slinky)
        {
            foreach (var segment in slinky.Segments)
            {
                Vector3 startPos = segment.position;
                Vector3 direction = Vector3.up;  // Burada herhangi bir yön kullanılabilir.
                direction.Normalize(); // Yönü normalize et

                float distance = 5f;

                RaycastHit[] hits = Physics.RaycastAll(startPos, direction, distance);
                foreach (var hit in hits)
                {
                    SlinkyController collidingSlinky = hit.collider.GetComponentInParent<SlinkyController>();

                    if (collidingSlinky == null || collidingSlinky == slinky) continue;

                    return true;
                }
            }
            return false;
        }
        public void ShiftRemainingSlinkies()
        {
            // Boş slotları al ve **en büyükten en küçüğe** sıralayalım
            List<GameKey> emptySlots = GetEmptySlots(false)
                .OrderByDescending(slot => slot.ToVector2Int().x) // X koordinatına göre azalan sırada (en büyükten en küçüğe)
                .ToList();

            // Debug: Boş slotların içeriğini yazdır
            Debug.Log("Boş Slotlar (emptySlots):");
            foreach (var slot in emptySlots)
            {
                Debug.Log($"Slot: {slot.ValueAsString} | X Koordinatı: {slot.ToVector2Int().x} | Y Koordinatı: {slot.ToVector2Int().y}");
            }

            // Alt griddeki tüm slinky'leri al
            List<SlinkyController> lowerGrids = GetAllSlinkiesInLowerGrid()
                .OrderByDescending(slinky => slinky.OccupiedGridKeys[0].ToVector2Int().x) // Slinky'leri X değerlerine göre azalan sırada sırala
                .ToList();

            // Debug: Slinky'lerin içeriğini yazdır
            Debug.Log("Alt Griddeki Slinky'ler (lowerGrids):");
            foreach (var slinky in lowerGrids)
            {
                Debug.Log($"Slinky: {slinky.name} | Slotlar: {string.Join(", ", slinky.OccupiedGridKeys.Select(k => k.ValueAsString))}");
            }

            int slotIndex = 0;

            // Boş slotlara slinky'leri yerleştir
            foreach (var slinky in lowerGrids)
            {
                // Eğer boş slotlar bitti ise, işlem sonlandırılacak
                if (slotIndex >= emptySlots.Count) break;

                // Slinkyin bulunduğu ilk slotu al
                GameKey currentSlot = slinky.OccupiedGridKeys[0];
                int currentX = currentSlot.ToVector2Int().x;

                // Boş slotları kontrol et
                GameKey targetSlot = emptySlots[slotIndex];
                int targetX = targetSlot.ToVector2Int().x;

                // Eğer slinky'nin X değeri, boş slotların X değerinden büyükse işlem yapma
                if (currentX >= targetX)
                {
                    Debug.Log($"Slinky'nin bulunduğu X değeri ({currentX}) boş slotun X değerinden ({targetX}) büyük. Kaydırma yapılmadı.");
                    continue; // Bu slinky için kaydırma işlemi yapılmasın
                }

                Vector3 targetPosition = GetSlotPosition(targetSlot, false);

                // Eğer slinky zaten bu hedef slotta (L_4,0 gibi) ise, işlem yapma
                if (slinky.OccupiedGridKeys.Contains(targetSlot))
                {
                    Debug.Log($"Slinky zaten {targetSlot.ValueAsString} slotunda, işlem yapılmadı.");
                    continue; // Bu slinky için bir şey yapma
                }

                // Önce mevcut slotundan kaldır
                _upperGrid.RemoveSlinky(slinky.OccupiedGridKeys[0]);

                // Sonra boş slotlara yerleştir
                _lowerGrid.PlaceSlinky(slinky, targetSlot);

                // Yeni yerleştirilen slotu güncelle
                slinky.OccupiedGridKeys.Clear();
                slinky.OccupiedGridKeys.Add(targetSlot);

                // Hedef slotu ve kaydırma işlemine dair loglama
                Debug.Log($"🎯 {slinky.name} → TargetSlot: {targetSlot.ValueAsString}");

                // Slinky'yi yeni hedefe doğru hareket ettir
                slinky.MoveToTarget(targetPosition, targetSlot);

                // Bir sonraki slotu hedeflemek için indexi artır
                slotIndex++;
            }
        }


        public void DebugLowerGridColors()
        {
            var allSlots = _lowerGrid.GetAllSlots();
            Dictionary<GameKey, string> colorMap = new Dictionary<GameKey, string>();

            foreach (var kvp in allSlots)
            {
                var slinky = _slinkies.FirstOrDefault(s => s.OccupiedGridKeys.Contains(kvp.Key));
                string slinkyColor = (slinky != null) ? slinky.SlinkyColor.ToString() : "unKnown";
                string slotStatus = (slinky != null) ? "Full" : "Empty";

                // Veriyi colorMap'e ekle
                colorMap[kvp.Key] = $"{slinkyColor} - {slotStatus}";

                // Debug: Slot bilgilerini yazdır
                Debug.Log($"Slot: {kvp.Key.ValueAsString} | Color: {slinkyColor} | Status: {slotStatus}");
            }

            // Eğer tüm veriyi bir arada görmek isterseniz
            Debug.Log("Color Map:");
            foreach (var item in colorMap)
            {
                Debug.Log($"Slot: {item.Key.ValueAsString}, Color: {item.Value}");
            }
        }


        public GameKey GetGridKeyFromPosition(Vector3 position)
        {
            foreach (var kvp in _upperGrid.GetAllSlots())
            {
                float distance = Vector3.Distance(kvp.Value.Position, position);

                if (distance < 0.1f)
                {
                    return kvp.Key;
                }
            }

            foreach (var kvp in _lowerGrid.GetAllSlots())
            {
                float distance = Vector3.Distance(kvp.Value.Position, position);

                if (distance < 0.1f)
                {
                    return kvp.Key;
                }
            }
            return null;
        }


        public List<GameKey> GetEmptySlotsInLowerGrid()
        {
            return _lowerGrid
                .GetAllSlots()
                .Where(kvp => !kvp.Value.IsOccupied)
                .OrderByDescending(kvp => kvp.Key.ToVector2Int().x) 
                .Select(kvp => kvp.Key)
                .ToList();
        }
        public List<SlinkyController> GetAllSlinkiesInLowerGrid()
        {
            return _slinkies.Where(slinky =>
                slinky.OccupiedGridKeys.Count > 0 &&
                slinky.OccupiedGridKeys.All(key => key.IsLower())
            ).ToList();
        }

    }

}
