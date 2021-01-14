
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

        postMessage(checkResult);
    });

    connection.on("SomeMessage_client_only", (someResult) => {

        postMessage({ update: someResult });
    });

    connection.on("SomeMessage_allexcept_client", (someResult) => {

        postMessage({ update: someResult });
    });


    connection.on("GroupMessageGeneral", (someResult) => {

        postMessage({ update: someResult });
    });


    connection.on("NewOrder", function (order) {

        postMessage(order, "Someone ordered an " + order.product);
    });


    function postMessage(order_or_checkResult, msg) {

        let order = order_or_checkResult;

        if (order_or_checkResult.order) order = order_or_checkResult.order;

        var statusDiv = document.getElementById("status");

        let divTempId = "divOrder" + order.orderNo;

        let divTemp = document.getElementById(divTempId);


        if (!divTemp) {

            divTemp = document.createElement("div");
            divTemp.className = "orderDiv";
            divTemp.id = divTempId;
            statusDiv.appendChild(divTemp);
            displayIfApplicable(statusDiv, divTemp, "Order No:" + order.orderNo);
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


document.getElementById("submit").addEventListener("click", e => {
    e.preventDefault();
    const product = document.getElementById("product").value;
    const size = document.getElementById("size").value;

    fetch("/Coffee",
        {
            method: "POST",
            body: JSON.stringify({ product, size }),
            headers: {
                'content-type': 'application/json'
            }
        })
        .then(response => response.text())
        .then(id => connection.invoke("GetUpdateForOrder", id));
});