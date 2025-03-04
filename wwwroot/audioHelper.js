window.setAudioSource = (url) => {
    let audio = document.getElementById("audioPlayer");
    let source = document.getElementById("audioSource");

    if (audio && source) {
        source.src = url;
        audio.load(); // Reloads the audio with new source
        audio.play(); // Plays the audio automatically
    }
};
window.downloadAudio = async (url, filename) => {
    try {
        let response = await fetch(url);
        let blob = await response.blob();
        let link = document.createElement("a");
        link.href = URL.createObjectURL(blob);
        link.download = filename;
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
    } catch (error) {
        console.error("Download failed:", error);
    }
};
