FMP7 Live Viewer (Unity Ver.)
=====
Copyright (C) 2017 TAN-Y

[最新バイナリはこちら (Latest Release)](https://github.com/aosoft/FMPLiveViewerUnity/releases/latest) | [更新履歴](update.md)  
[ライセンス (MIT)](LICENSE.txt)

## このアプリケーションについて

[FMP7 SDK for .NET](https://github.com/aosoft/FMP7ApiCLR) で本当に Unity アプリが作れるかの実証サンプルです。あと UniRx の研究。  

以前、 FMP7 のライブイベント用に作った Viewer っぽいものを Unity で作り直すことを目標にしています。当時は基本 WPF (最終の 2013 年版のみ WPF + Direct3D11 ネイティブのハイブリッド) でした。  
Viewer としてはまだまだですが、 Unity アプリのサンプルとしては一応達成できていると思いますので、とりあえず公開します。

## Unity のバージョン

5.5.1f1 (特にこだわっているわけではありません)


## 動かし方

"StreamingAssets" フォルダ下に FMP7 の曲データを配置してください。 (ビルド済バイナリでも StreamingAssets の中身を変えてしまって OK です)

起動すると "Next Music" と "Play" ボタンが表示されます。操作できるものは今のところこれだけです。

| ボタン | 動作 |
|:--|:--|
| Next Music | StreamingAssets 内の曲データを順次切り替えて再生開始する。 |
| Play (Pause) | 現在ロードされている曲の再生／一時停止 |

FMP7 は起動していないと操作できません。

## コードのポイント

Assets/Scripts/UnityFMP 下の RxFMPWork.cs, RxFMPPartWork.cs が FMP のワーク監視用の Reactive 関連実装です。ここのもの必要に応じて Subscribe して利用してください。足りないパラメーターは似たような感じで ReactiveProperty を定義すればよいと思います。

RxFMPWork はシングルトンで保持をし、どこかの MonoBehavior の OnUpdate から定期的に Update メソッドを呼び出して内容を更新してください。このアプリでは FMPManager クラスで行っています (FMPManager では UpdateAsObservable を使っています) 。

FMP のバージョンやワークサイズの情報は FMPMessageListener を用いて FMP 起動のタイミングで取得し、 FMPWork にアクセスする際には取得済のものを利用するようにしています。 FMPWork 取得時に毎回ワーク情報を取得しようとすると、巨大な PCM のデータをロードする際、一瞬フリーズしてしまうので注意が必要です。

FMP の制御、連携以外については特に変わった事はしていません。

## ライセンス

MIT License です。

利用させていただいている下記コンポーネントはそれぞれのライセンスに従います。

* [UniRx](https://github.com/neuecc/UniRx) (MIT License)
* [FMP7 SDK for .NET](https://github.com/aosoft/FMP7ApiCLR) (zlib/libpng License)
