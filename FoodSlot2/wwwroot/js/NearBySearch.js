async function testApi(keyword, latitude, longitude, radius) {

    const response =
        await fetch('/StoreSearch/Nearby', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },

            body: JSON.stringify({
                keyword: keyword,
                latitude: latitude, 
                longitude: longitude,
                radius: radius
            })
        });

    const stores =
        await response.json();

    console.log(stores);

    let html = '';

    stores.forEach(store => {

        html += `
                <div>
                    <h3>${store.name}</h3>
                    <p>${store.address}</p>
                    <p>${store.rating}</p>
                    <img src="${store.photoUrl}"/>
                    <a href="${store.googleMapUrl}" target="_blank" rel="noopener noreferrer">
                    導航
                    </a>
                </div>
            `;
    });
    /*target="_blank" 開新分頁 */
    /*rel="noopener noreferrer" 安全性保護*/
    document
        .getElementById('result')
        .innerHTML = html;
}