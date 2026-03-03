const connection = new signalR.HubConnectionBuilder()
    .withUrl("/audio")
    .build();

connection.on("ReceiveAudio", (data) => {
    // 创建音频上下文
    const audioContext = new (window.AudioContext || window.webkitAudioContext)();
    
    // 解码服务器返回的音频数据
    audioContext.decodeAudioData(data.buffer).then(buffer => {
        // 创建音频源节点
        const source = audioContext.createBufferSource();
        source.buffer = buffer;
        
        // 添加音量控制
        const gainNode = audioContext.createGain();
        gainNode.gain.value = 1.0;
        
        // 连接节点
        source.connect(gainNode);
        gainNode.connect(audioContext.destination);
        
        // 播放音频
        source.start(0);
        
        // 音频分析处理
        const analyser = audioContext.createAnalyser();
        analyser.fftSize = 2048;
        gainNode.connect(analyser);
        
        // 实时音频分析
        const bufferLength = analyser.frequencyBinCount;
        const dataArray = new Uint8Array(bufferLength);
        
        function visualize() {
            analyser.getByteTimeDomainData(dataArray);
            // 这里可以添加可视化逻辑
            requestAnimationFrame(visualize);
        }
        visualize();
    });
});

connection.start().then(() => {
    navigator.mediaDevices.getUserMedia({ audio: true })
        .then(stream => {
            const audioContext = new AudioContext();
            
            // 优先尝试使用AudioWorklet
            if (audioContext.audioWorklet) {
                audioContext.audioWorklet.addModule('audio-processor.js')
                    .then(() => {
                        const workletNode = new AudioWorkletNode(
                            audioContext, 
                            'audio-processor');
                        
                        const source = audioContext.createMediaStreamSource(stream);
                        source.connect(workletNode);
                        
                        workletNode.port.onmessage = (e) => {
                            connection.invoke("StreamAudio", 
                                e.data.buffer, 
                                48000, 16, 1);
                        };
                    });
            } 
            // 降级使用ScriptProcessorNode
            else {
                const processor = audioContext.createScriptProcessor(4096, 1, 1);
                
                processor.onaudioprocess = (e) => {
                    const data = e.inputBuffer.getChannelData(0);
                    connection.invoke("StreamAudio", data, 48000, 16, 1);
                };
                
                const source = audioContext.createMediaStreamSource(stream);
                source.connect(processor);
                processor.connect(audioContext.destination);
            }
        });
});