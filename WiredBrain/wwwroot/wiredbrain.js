// WebSocket = undefined;
//EventSource = undefined;
//, signalR.HttpTransportType.LongPolling

let connection = null;

setupConnection = () => {
    connection = new signalR.HubConnectionBuilder()
        .withUrl("/coffeehub")
        .build();

    // 01/12/2021 06:21 am - SSN - [20210112-0607] - [005] - M04-02 - Implementing a hub 
    // connection.on("ReceiveOrderUpdate", (update) => {
    connection.on("ReceiveOrderUpdate", (checkResult) => {

        postMessage(checkResult);
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

        console.log('20210113-0240');
        console.log(order_or_checkResult);
        if (order_or_checkResult.finished) {
            console.error("Finished:");
            className = "green";
            addClick = true
        }

        console.log("20210112-0726");
        console.log("-------------");
        console.log(order_or_checkResult);
        console.log(order);

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