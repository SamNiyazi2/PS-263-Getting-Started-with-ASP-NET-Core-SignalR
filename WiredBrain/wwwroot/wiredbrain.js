
// 01/13/2021 07:04 am - SSN - [20210113-0704] - [001] - M04-08 - Transport negotiation

//WebSocket = undefined;
//EventSource = undefined;
//, signalR.HttpTransportType.LongPolling


let connection = null;


// 01/13/2021 07:16 am - SSN - [20210113-0704] - [002] - M04-08 - Transport negotiation
//    .withUrl("/coffeehub")
//    .withUrl("/coffeehub", signalR.HttpTransportType.LongPolling)

console.log('20210114-0506');
console.log(signalR.HttpTransportType.None);
console.log(signalR.HttpTransportType.WebSockets);
console.log(signalR.HttpTransportType.ServerSentEvents);
console.log(signalR.HttpTransportType.LongPolling);

setupConnection = () => {
    connection = new signalR.HubConnectionBuilder()
        // .withUrl("/coffeehub", signalR.HttpTransportType.ServerSentEvents)
        .withUrl("/coffeehub")
        .build();

    // 01/12/2021 06:21 am - SSN - [20210112-0607] - [005] - M04-02 - Implementing a hub 
    // connection.on("ReceiveOrderUpdate", (update) => {
    connection.on("ReceiveOrderUpdate", (checkResult) => {

        postMessage(checkResult, "on ReceiveOrderUpdate message");
    });

    connection.on("SomeMessage_client_only", (someResult) => {

        postMessage({ update: someResult }, "on SomeMessage_client_only");
    });

    connection.on("SomeMessage_allexcept_client", (someResult) => {

        postMessage({ update: someResult });
    });


    connection.on("GroupMessageGeneral", (someResult) => {

        postMessage({ update: someResult }, "on GroupMessageGeneral");

    });


    connection.on("NewOrder", function (order) {

        postMessage(order, "Order for: " + order.product);
    });


    function postMessage(order_or_checkResult, msg) {

        if (order_or_checkResult == undefined) {
            return;
        }

        let order = order_or_checkResult;

        if (order_or_checkResult.order) order = order_or_checkResult.order;

        var statusDiv = document.getElementById("status");

        let divTempId = "divOrder" + order.orderNo;

        let divTemp = document.getElementById(divTempId);


        if (!divTemp) {

            divTemp = document.createElement("div");
            divTemp.className = "orderDiv";
            divTemp.id = divTempId;
            // 09/26/2022 08:37 pm - SSN - insertAdjacentElement instead of appendChild
            statusDiv.insertAdjacentElement('afterbegin', divTemp);

            if (order && order.orderNo) {
                displayIfApplicable(statusDiv, divTemp, "Order No:" + order.orderNo);
            }
        }

        let className = "blue";
        let addClick = false;


        if (order_or_checkResult.finished) {

            className = "green";
            addClick = true
        }

        displayIfApplicable(statusDiv, divTemp, order_or_checkResult.update, className, addClick);
        displayIfApplicable(statusDiv, divTemp, msg);

    }


    function displayIfApplicable(statusDiv, divTemp, stringContent, className, addClick) {

        if (stringContent) {

            //divTemp.insertAdjacentHTML('beforeend', stringContent);
            //divTemp.insertAdjacentHTML('beforeend', "<br/>");

            let p = document.createElement("p");
            p.innerText = stringContent;
            p.className = className;
            divTemp.appendChild(p);

            if (addClick) {

                p.title = "Click to remove.";

                p.addEventListener('click', (e) => {
                    statusDiv.removeChild(divTemp);
                })
            }
        }
    }





    connection.on("finished", function () {
        connection.stop();
    }
    );

    connection.start()
        .catch(err => console.error(err.toString()));
};


setupConnection();


// 11/13/2022 03:35 pm - SSN - Revise - Validate input

document.getElementById("submit").addEventListener("click", e => {
    e.preventDefault();


    idProductError.innerText = "";
    idProductError.className = "";

    idSizeError.innerText = "";
    idSizeError.className = "";

    let errorCount = 0;

    if (size.selectedIndex <= 0) {
        idSizeError.innerText = "Input is required.";
        idSizeError.className = "cssError alert-danger";
        size.focus();
        errorCount++;
    }

    if (product.selectedIndex <= 0) {
        idProductError.innerText = "Input is required.";
        idProductError.className = "cssError alert-danger";
        product.focus();
        errorCount++;
    }

    if (errorCount > 0) return;

    let sizeValue = size.options[size.selectedIndex].text
    let productText = product.options[product.selectedIndex].text



    fetch("/Coffee",
        {
            method: "POST",
            body: JSON.stringify({ product: productText, size: sizeValue }),
            headers: {
                'content-type': 'application/json'
            }
        })
        .then(response => {

            let results = response.text();
            return results;
        }

        )
        .then(id => {


            let results = connection.invoke("GetUpdateForOrder", id);
            return results;

        });
});


// 11/13/2022 03:56 am - SSN - Change free text input to select box.

const coffeeDrinkOptions = [
    'Raspberry Green Tea',
    'Cookie monster frappe',
    'Peanut butter cup frappe',
    'Nutty Brunette frappe',
    'Iced black tea',
    'Iced latte nude flavor',
    'Iced latte white ninja flavor',
    'Iced latte tuxedo flavor'
];


const sizeOptions = [
    'Small',
    'Medium',
    'Large'
];


function addSelectOption(listObj, selectIndex, innerText) {

    let option1 = new Option();
    option1.value = selectIndex;
    option1.innerText = innerText;
    listObj.options[selectIndex] = option1;

}


function fillList(listObj, dataArray) {

    addSelectOption(listObj, 0, 'Make a selection');

    for (let ndx = 0; ndx < dataArray.length; ndx++) {

        addSelectOption(listObj, ndx + 1, dataArray[ndx]);

    }

}



(function () {

    fillList(product, coffeeDrinkOptions);
    fillList(size, sizeOptions);

})();

