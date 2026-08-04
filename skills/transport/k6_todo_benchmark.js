import http from 'k6/http';
import { check, sleep } from 'k6';
import { Counter, Trend, Rate } from 'k6/metrics';

// k6 run k6_todo_benchmark.js
// 定义性能指标
const todoCreateCounter = new Counter('todo_create_count');
const todoReadCounter = new Counter('todo_read_count');
const todoUpdateCounter = new Counter('todo_update_count');
const todoDeleteCounter = new Counter('todo_delete_count');
const todoLatency = new Trend('todo_latency');
const todoErrorRate = new Rate('todo_error_rate');

// 新增P99延迟指标
const todoCreateP99 = new Trend('todo_create_p99_latency');
const todoReadP99 = new Trend('todo_read_p99_latency'); 
const todoUpdateP99 = new Trend('todo_update_p99_latency');
const todoDeleteP99 = new Trend('todo_delete_p99_latency');

// 更新配置选项
export const options = {
    stages: [
        { duration: '30s', target: 100 },  // 预热阶段
        { duration: '1m', target: 500 },   // 负载阶段
        { duration: '30s', target: 1000 },  // 峰值阶段
        { duration: '30s', target: 0 },     // 冷却阶段
    ],
    thresholds: {
        http_req_duration: [
            'p(95)<500',  // 95%请求延迟<500ms
            'p(99)<1000'  // 新增P99延迟要求
        ],
        'todo_error_rate': ['rate<0.1'],
        'todo_create_p99_latency': ['p(99)<800'],
        'todo_read_p99_latency': ['p(99)<600'],
        'todo_update_p99_latency': ['p(99)<700'],
        'todo_delete_p99_latency': ['p(99)<500']
    }
};

// 更新测试逻辑
export default function () {
    // 创建Todo测试
    const createRes = http.post(`${baseUrl}/todo`, createPayload, { headers });
    check(createRes, {
        'create status 201': (r) => r.status === 201,
    });
    todoCreateCounter.add(1);
    todoLatency.add(createRes.timings.duration);
    if (createRes.status !== 201) {
        todoErrorRate.add(1);
    }
    todoCreateP99.add(createRes.timings.duration); // 记录P99延迟

    // 读取Todo测试
    const readRes = http.get(`${baseUrl}/todo/${todoId}`, { headers });
    check(readRes, {
        'read status 200': (r) => r.status === 200,
    });
    todoReadCounter.add(1);
    todoLatency.add(readRes.timings.duration);
    if (readRes.status !== 200) {
        todoErrorRate.add(1);
    }
    todoReadP99.add(readRes.timings.duration);

    // 更新Todo测试 
    const updateRes = http.put(`${baseUrl}/todo`, updatePayload, { headers });
    check(updateRes, {
        'update status 200': (r) => r.status === 200,
    });
    todoUpdateCounter.add(1);
    todoLatency.add(updateRes.timings.duration);
    if (updateRes.status !== 200) {
        todoErrorRate.add(1);
    }
    todoUpdateP99.add(updateRes.timings.duration);

    // 删除Todo测试
    const deleteRes = http.del(`${baseUrl}/todo/${todoId}`, null, { headers });
    check(deleteRes, {
        'delete status 204': (r) => r.status === 204,
    });
    todoDeleteCounter.add(1);
    todoLatency.add(deleteRes.timings.duration);
    if (deleteRes.status !== 204) {
        todoErrorRate.add(1);
    }
    todoDeleteP99.add(deleteRes.timings.duration);

    sleep(1); // 思考时间
}