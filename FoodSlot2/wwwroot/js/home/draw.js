// 全域變數定義
let map;                    // Google 地圖實例
let mapMarkers = [];        // 存放目前地圖上的餐廳標記
let currentInfoWindow = null;// 目前打開的氣泡窗
let isDrawing = true;       //防連點鎖定狀態：預設先鎖定，等 SDK 載入完成才解鎖，防止連點與流暢度崩潰
const ITEM_HEIGHT = 325;    /* 配合 CSS .slot-item 的高度 */

// 預設會員(userID=1)的備用地點：以台北車站為例 (緯度, 經度)
const DEFAULT_LAT = 25.0478;
const DEFAULT_LNG = 121.5170;

// 網頁初始化
document.addEventListener("DOMContentLoaded", async () => {
    // 1. 初始化抽籤按鈕外觀狀態（防呆）
    const drawBtn = document.getElementById('drawButton');
    drawBtn.disabled = true;
    drawBtn.style.cursor = 'not-allowed';

    // 2. 頁面一開啟，先去後端抓取該使用者的初始獎池項目
    await initializePrizePool();

    // 3. 向後端索取 APIKey，並動態載入 Google Map 載具
    await loadGoogleMapsSdkAsync();

    // 4. 綁定拉霸按鈕點擊事件
    drawBtn.addEventListener('click', startDraw);
});

// 首次載入：只獲取獎池清單，不觸發抽籤
async function initializePrizePool() {
    try {
        // 呼叫讀取獎池的API
        const response = await fetch('/API/Draw/GetPrizePool', { method: 'GET' });
        if (!response.ok) throw new Error('初始化獎池失敗');

        const foodsPool = await response.json();// 這邊直接就是 List<VMFoodSlotItem> 陣列

        // 渲染初始化的食物清單到羊皮紙上
        renderParchmentPool(foodsPool);
    } catch (error) {
        console.error("初始化獎池錯誤:", error);
        document.getElementById('poolScrollContainer').innerHTML = '<p style="color:red; padding-top:20px;">暫無獎池資料</p>';
    }
}

// 動態載入 Google Maps API
async function loadGoogleMapsSdkAsync() {
    try {
        // 跟後端要 Key
        const response = await fetch('/api/GoogleMapsAPI/GetApiKey');
        const data = await response.json();

        // 用程式碼動態建立 <script> 標籤
        const script = document.createElement('script');
        script.src = `https://maps.googleapis.com/maps/api/js?key=${data.apiKey}&libraries=places&language=zh-TW`;
        script.async = true;
        script.defer = true;

        // 【重點】當 Script 真正載入完成後，才把抽籤按鈕解鎖
        script.onload = () => {
            console.log("Google Maps SDK 動態載入成功！");
            const drawBtn = document.getElementById('drawButton');
            unlockButton(drawBtn);
        };

        document.head.appendChild(script);
    } catch (error) {
        console.error("無法載入 Google Maps SDK:", error);
    }
}

// 核心流程：點擊開始抽籤
async function startDraw() {
    if (isDrawing) return;

    // 1. 鎖定按鈕狀態
    isDrawing = true;
    const drawBtn = document.getElementById('drawButton');
    drawBtn.disabled = true;
    drawBtn.style.cursor = 'not-allowed';

    // 2. 切換畫面：如果地圖開著，先藏起來，換回拉霸容器
    document.getElementById('googleMap').style.display = 'none';
    document.getElementById('slotReelContainer').style.display = 'flex';

    try {
        // 3. 呼叫後端抽籤
        const response = await fetch('/API/Draw/StartDraw', { method: 'POST' });
        if (!response.ok) throw new Error('抽籤失敗');
        const data = await response.json();

        console.log("抽籤結果：", data);

        // 4. 準備拉霸輪盤圖片
        renderReel(data.reelItems);

        // 5. 執行拉霸動畫（滾動）
        await playReelAnimation(data.reelItems);

        // 6. 動態更新羊皮紙為「中獎公告」
        renderWinningResult(data.selectedFood);

        // 7. 延遲 3 秒後，自動轉場至 Google 地圖與搜尋
        setTimeout(() => {
            switchToGoogleMapFlow(data.selectedFood.foodname);
        }, 3000);

    } catch (error) {
        console.error(error);
        alert('抽籤過程發生錯誤');
        unlockButton(drawBtn);
    }
}

// 畫面渲染與動畫功能
// 渲染拉霸輪盤
function renderReel(reelItems) {
    const reel = document.getElementById('slotReel');
    reel.innerHTML = '';
    // 重設滾動位置到最頂端
    reel.style.transition = 'none';
    reel.style.transform = 'translateY(0px)';

    reelItems.forEach(food => {
        reel.innerHTML += `
                <div class="slot-item">
                    <img src="${food.photoUrl}" alt="${food.foodname}" />
                </div>
            `;
    });
}
// 執行拉霸滾動動畫（CSS Transition）
function playReelAnimation() {
    return new Promise(async (resolve) => {
        const reel = document.getElementById('slotReel');
        if (!reel) return resolve();

        // 在所有邏輯開始前，強制移除 Transition，並瞬間把位置歸零到最頂端(第一張圖)
        reel.style.transition = 'none';
        reel.style.transform = 'translateY(0px)';

        // 強制瀏覽器立刻執行這一次重繪 (Reflow)，確保輪盤「現在」就在 0px 的位置
        reel.offsetHeight;

        // 動態取得目前輪盤內實際的圖片總數，避免寫死而發生誤差
        const itemCount = reel.children.length;
        if (itemCount === 0) return resolve();

        console.log("等待 30 張圖片載入中...");
        // 等待所有圖片載入完成
        const imagesInReel = reel.querySelectorAll('img');
        const loadImagePromises = Array.from(imagesInReel).map(img => {
            return new Promise((imgResolve) => {
                // 如果圖片已經載入完成（緩存），立刻 resolve
                if (img.complete) {
                    imgResolve();
                } else {
                    // 否則等待 load 事件
                    img.addEventListener('load', imgResolve);
                    // 如果圖片跑不動 (error)，也要 resolve，以免卡死動畫
                    img.addEventListener('error', imgResolve);
                }
            });
        });
        // 等待所有圖片 Promise 完成（全部撐開容器高度）
        await Promise.all(loadImagePromises);
        console.log("30 張圖片全部載入完成，容器高度確定。準備開始動畫。");

        // 圖片載入完畢後，容器高度變了，必須再次強制瀏覽器重繪，
        // 確保在設定新的動畫軌跡前，初始狀態依然牢牢鎖在 translateY(0px)。
        reel.offsetHeight;

        // 移動到最後一張圖的頂部
        const totalMove = (itemCount - 1) * ITEM_HEIGHT;

        // 加上滑順的滾動動畫
        const animationTime = 8; // 你設定的時間，單位：秒
        reel.style.transition = `transform ${animationTime}s cubic-bezier(0.42, 0, 0.58, 1)`;
        reel.style.transform = `translateY(-${totalMove}px)`;

        // 動態結束時 resolve，並且「不做」任何歸零動作，讓它牢牢停在原地
        setTimeout(resolve, animationTime * 1000);
    });
}

// 渲染初始獎池（食物種類列表）
function renderParchmentPool(foodsPool) {
    const container = document.getElementById('poolScrollContainer');
    if (!container) return;
    container.innerHTML = '';
    foodsPool.forEach(food => {
        container.innerHTML += `
        <div class="pool-item">
            <img src="${food.photoUrl}" alt="${food.foodname}" />
            <p>${food.foodname}</p>
        </div>
        `;
    });
}
// 渲染中獎公告
function renderWinningResult(selectedFood) {
    const container = document.getElementById('poolScrollContainer');
    container.innerHTML = `
            <div class="winning-card" style="text-align:center; padding-top:40px;">
                <h1 style="color:#5b3417; font-size:28px;">恭喜抽中</h1>
                <img src="${selectedFood.photoUrl}" style="width:160px; height:160px; border-radius:15px; margin:20px 0; box-shadow:0 4px 8px rgba(0,0,0,0.2);" />
                <h2 style="color:#d32f2f; font-size:32px; margin:0;">${selectedFood.foodname}</h2>
                <p style="color:#795548; margin-top:15px; font-weight:bold;">正在搜尋附近餐廳...</p>
            </div>
            `;
}

// 地圖切換與 Google API 串接流程
async function switchToGoogleMapFlow(keyword) {
    // 1. 隱藏拉霸機畫面，顯示地圖容器
    document.getElementById('slotReelContainer').style.display = 'none';
    const mapScreen = document.getElementById('googleMap');
    mapScreen.style.display = 'block';
    // 2. 啟動定位防禦機制（GPS vs 預設會員位置）
    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(
            async (position) => {
                // 使用者允許定位
                await executeNearbySearchWorkflow(keyword, position.coords.latitude, position.coords.longitude);
            },
            async (error) => {
                // 使用者拒絕定位，改用預設會員地點
                console.warn("地理定位被拒絕或失敗，改用預設會員位置。");
                await executeNearbySearchWorkflow(keyword, DEFAULT_LAT, DEFAULT_LNG);
            }
        );
    } else {
        // 瀏覽器不支援定位
        await executeNearbySearchWorkflow(keyword, DEFAULT_LAT, DEFAULT_LNG);
    }
}
// 執行附近商家搜尋並初始化地圖元件
async function executeNearbySearchWorkflow(keyword, lat, lng) {
    // 1. 初始化 Google 地圖 (鎖定在拉霸機螢幕內)
    const mapOptions = {
        center: { lat: lat, lng: lng },
        zoom: 15,
        mapTypeControl: false,
        streetViewControl: false
    };
    map = new google.maps.Map(document.getElementById('googleMap'), mapOptions);
    currentInfoWindow = new google.maps.InfoWindow();
    // 清除之前的舊標記
    clearMarkers();

    try {
        // 2. 呼叫你的 StoreSearchController
        const response = await fetch('/StoreSearch/Nearby', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
                keyword: keyword,
                latitude: lat,
                longitude: lng,
                radius: 3000    // 搜尋半徑 3 公里
            })
        });

        const stores = await response.json();
        console.log("附近餐廳搜尋結果：", stores);
        // 3. 處理搜尋結果與連動
        renderStoreListToParchment(stores);
        createStoreMarkersOnMap(stores);

    } catch (error) {
        console.error("搜尋附近餐廳失敗:", error);
    } finally {
        // 流程全部結束，解鎖按鈕
        unlockButton(document.getElementById('drawButton'));
    }
}
// 渲染附近的餐廳清單到羊皮紙上
function renderStoreListToParchment(stores) {
    const container = document.getElementById('poolScrollContainer');
    if (!container) return;
    container.innerHTML = '';

    if (stores.length === 0) {
        container.innerHTML = '<p style="color:#5b3417; font-weight:bold; padding-top:50px; text-align:center;">附近沒有找到相關餐廳</p>';
        return;
    }

    stores.forEach((store, index) => {
        const imgUrl = store.photoUrl ? store.photoUrl : '/images/default-store.png';

        // 處理營業時間狀態 (配合 Google API 的 open_now 邏輯，若後端 DTO 有串接可以對應；若無則先顯示預設提示)
        // 這裡假設你未來可能會從後端擴充 StoreDTO 的 OpenNow 屬性，我們先做防呆：
        const openStatusHtml = store.openNow !== undefined
            ? (store.openNow ? '<span style="color:green; font-weight:bold;">🟢 營業中</span>' : '<span style="color:red; font-weight:bold;">🔴 休息中</span>')
            : '<span style="color:#795548;">詳情請見地圖</span>';

        // 構建橫向卡片 HTML
        container.innerHTML += `
            <div class="store-card" data-index="${index}" onclick="focusStoreOnMap(${index})" 
                 style="display: flex; background: rgba(255, 255, 255, 0.85); border-radius: 12px; margin-bottom: 15px; padding: 12px; box-shadow: 0 4px 6px rgba(0,0,0,0.1); cursor: pointer; transition: transform 0.2s;">
                
                <div class="store-card-left" style="flex-shrink: 0; margin-right: 12px;">
                    <img src="${imgUrl}" alt="${store.name}" style="width: 90px; height: 90px; border-radius: 8px; object-fit: cover;" />
                </div>
                
                <div class="store-card-right" style="flex-grow: 1; display: flex; flex-direction: column; justify-content: space-between; text-align: left;">
                    <div style="margin: 0;">
                        <h4 style="font-size: 18px; color: #5b3417; margin: 0 0 4px 0; font-weight: bold; line-height: 1.3;">${store.name}</h4>
                        <p style="font-size: 13px; color: #666; margin: 0 0 4px 0; line-height: 1.2;">📍 ${store.address}</p>
                    </div>
                    <div style="display: flex; justify-content: space-between; align-items: center; font-size: 13px; margin-top: 5px;">
                        <span style="color: #ff9800; font-weight: bold;">⭐ ${store.rating.toFixed(1)}</span>
                        ${openStatusHtml}
                    </div>
                </div>
                
            </div>
        `;
    });
}
// 在地圖上放置餐廳標籤（Markers）
function createStoreMarkersOnMap(stores) {
    stores.forEach((store, index) => {
        const marker = new google.maps.Marker({
            position: { lat: store.latitude, lng: store.longitude },
            map: map,
            title: store.name,
            animation: google.maps.Animation.DROP
        });

        // 把當前的 store 資料直接綁在 marker 上，避免 focusStoreOnMap 找不到資料
        marker.storeData = store;

        // 點擊地圖上的地標彈出氣泡
        marker.addListener('click', () => {
            //直接傳入 marker 以及剛剛綁定的屬性
            openInfoWindowForStore(marker, marker.storeData);
        });
        // 存入全域陣列以便後續連動
        mapMarkers.push(marker);
    });
}

// 【核心連動】當使用者點擊羊皮紙餐廳時，地圖移動並彈出氣泡
window.focusStoreOnMap = function (index) {
    const marker = mapMarkers[index];

    if (marker) {
        // 1. 地圖中心移動到該餐廳
        map.panTo(marker.getPosition());
        map.setZoom(16);

        // 2.直接觸發該地標的點擊事件，它會自動抓到對應的 storeData 渲染 InfoWindow
        google.maps.event.trigger(marker, 'click');
    }
};
// 開啟氣泡視窗
function openInfoWindowForStore(marker, store) {
    if (currentInfoWindow) currentInfoWindow.close();

    const contentString = `
            <div style="color:#333; font-family:sans-serif; padding:5px; max-width: 200px;">
                <h4 style="margin:0 0 5px 0; color:#5b3417; font-size:14px;">${store.name}</h4>
                <p style="margin:0 0 5px 0; font-size:12px; color:#666;">${store.address}</p>
                <p style="margin:0 0 5px 0; font-size:12px; color:#ff9800;">評分：⭐ ${store.rating}</p>
                <a href="${store.googleMapUrl}" target="_blank" rel="noopener noreferrer"
                    style="display:inline-block; margin-top:5px; padding:4px 8px; background:#b5885c; color:white; text-decoration:none; border-radius:4px; font-size:11px;">
                    在 Google 地圖打開
                </a>
            </div>
            `;
    currentInfoWindow.setContent(contentString);
    currentInfoWindow.open(map, marker);
}
// 清除舊地標
function clearMarkers() {
    mapMarkers.forEach(m => m.setMap(null));
    mapMarkers = [];
}
// 解鎖按鈕
function unlockButton(btn) {
    isDrawing = false;
    btn.disabled = false;
    btn.style.cursor = 'pointer';
}