// 1. 连接到 SSE 端点
const source = newEventSource('http://localhost:5000/stocks');

// 2. 监听我们命名的 "stockUpdate" 事件
source.addEventListener('stockUpdate', e => {
    // 解析 JSON 负载
    const { symbol, price, timestamp } = JSON.parse(e.data);

    // 创建并前置一个带有 Tailwind 类的新列表项
    const li = document.createElement('li');
    li.classList.add('new', 'flex', 'justify-between', 'items-center');

    // 创建时间元素
    const timeSpan = document.createElement('span');
    timeSpan.classList.add('text-gray-500', 'text-sm');
    timeSpan.textContent = newDate(timestamp).toLocaleTimeString();

    // 创建符号元素
    const symbolSpan = document.createElement('span');
    symbolSpan.classList.add('font-medium', 'text-gray-800');
    symbolSpan.textContent = symbol;

    // 创建价格元素
    const priceSpan = document.createElement('span');
    priceSpan.classList.add('font-bold', 'text-green-600');
    priceSpan.textContent = `$${price}`;

    // 将所有元素附加到列表项
    li.appendChild(timeSpan);
    li.appendChild(symbolSpan);
    li.appendChild(priceSpan);

    const list = document.getElementById('updates');
    list.prepend(li);

    // 片刻后移除高亮
    setTimeout(() => li.classList.remove('new'), 2000);
});

// 3. 处理错误和自动重连
source.onerror = err => {
    console.error('SSE connection error:', err);
};

// 4. (可选) 检查最后收到的事件 ID
source.onmessage = e => {
    console.log('Last Event ID now:', source.lastEventId);
};