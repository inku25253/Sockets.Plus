using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.Sockets.Plus.HttpPackets
{
	public enum HttpStatus : int
	{
		/// <summary>
		/// 継続 
		/// クライアントはリクエストを継続できる。サーバーリクエストの最初の部分を受け取り、まだ拒否していないことを示す。
		/// 例として、クライアントがExpect: 100-Continue ヘッダをつけたリクエストを行い、それをサーバーが受理した場合に返される。
		/// </summary>
		Continue							= 100,
		/// <summary>
		/// プロトコル切り替え
		/// サーバーはリクエストを理解し、遂行のためにプロトコルの切り替えを要求している。
		/// </summary>
		SwitchingProtocols					= 101,
		/// <summary>
		/// 処理中
		/// WebDavの拡張ステータスコード。処理が継続されて行われていることを示す。
		/// </summary>
		Processing							= 102,
		CheckPoint							= 103,
		//RequestUriTooLong					= 122,


		/// <summary>
		/// OK
		/// リクエストは成功し、レスポンスとともに要求に応じた情報が返される。
		/// ブラウザでページが正しく表示された場合は、ほとんどがこのステータスコードを返している。
		/// </summary>
		OK									= 200,
		/// <summary>
		/// 作成
		/// リクエストは完了し、新たに作成されたリソースのURIが返される。
		/// 例: PUTメソッドでリソースを作成するリクエストっを行ったとき、そのリクエストが完了した場合に返される。
		/// </summary>
		Created								= 201,
		/// <summary>
		/// 受理 
		/// リクエストは受理したが、処理は完了していない。
		/// 例: PUTメソッドでリソースを作成するリクエストを行った時、サーバーがリクエストを受理したものの、リソースの作成が完了していない場合に返される。バッチ処理向け。
		/// </summary>
		Accepted							= 202,
		/// <summary>
		/// 信頼出来ない情報
		/// オリジナルのデータではなく、ローカルやプロキシ等からの情報であることを示す。
		/// </summary>
		NoAuthoritativeInformation			= 203,
		/// <summary>
		/// 内容なし
		/// リクエストを受理したが、返すべきレスポンスエンティティが存在しない場合に返される。
		/// 例: POSTメソッドでフォーラムの内容を送信したが、ブラウザの画面を更新しない場合に返される。
		/// </summary>
		NoContent							= 204,
		/// <summary>
		/// 内容のリセット
		/// リクエストを受理し、ユーザーエージェントの画面をリセットする場合に返される。
		/// 例: POSTメソッドでフォーラムの内容を送信した後、ブラウザの画面を初期状態に戻す場合に返される。
		/// </summary>
		ResetContent						= 205,
		/// <summary>
		/// 部分的内容
		/// 部分的GETリクエストを受理した時に返される。
		/// 例: ダウンロードツール等で分割ダウンロードを行った場合や、レジュームを行った場合に返される。
		/// </summary>
		PartialContent						= 206,
		/// <summary>
		/// 複数のステータス
		/// WebDavの拡張ステータスコード
		/// </summary>
		MultiStatus							= 207,
		AlreadyReported						= 208,
		/// <summary>
		/// IM仕様
		/// Delta Encoding In Httpの拡張ステータスコード。
		/// </summary>
		ImUsed								= 226,


		/// <summary>
		/// 複数の選択
		/// リクエストしたリソースが複数存在し、ユーザーやユーザーエージェントに選択肢を提示するときに返される。
		/// 具体例: http://www.w3.org/TR/xhtml11/DTD/xhtml11.html
		/// </summary>
		MultipleChoices						= 300,
		/// <summary>
		/// 恒久的に移動した
		/// リクエストしたリソースが恒久的に移動されているときに返される。Location: ヘッダに移動先のURLが示される。
		/// 例としてはファイルではなくディレクトリに対応するURLの末尾に/を書かずにアクセスした場合に返される。
		/// 具体例: http://www.w3.org/TR
		/// </summary>
		MovedPermanently					= 301,
		/// <summary>
		/// 発見した
		/// リクエストしたリソースが一時的に移動されているときに返される。Location: ヘッダに移動先のURLが示される。
		/// 元々はMoved Temporarily(一時的に移動した)で、本来はリクエストしたリソースが一時的にそのURLに存在せず、別のURLにある場合に仕様するステータスコードであった。しかし、例えば掲示板やWikiなどで投稿後にブラウザを他のURLに転送したいときにもこのコードが仕様されるようになったため、302はFoundになり、新たに303(SeeOther),307(TemporaryRedirect)が作成された。
		/// </summary>
		Found								= 302,
		/// <summary>
		/// 他を参照せよ
		/// リクエストに対するレスポンスが他のURLに存在するときに返される。Location: ヘッダに移動先のURLが示されている。
		/// リクエストしたリソースは確かにそのURLにあるが、他のリソースをもってレスポンスとするような場合に仕様する。302の説明で挙げたような、掲示板やWikiなどで投稿後にブラウザを他のURLに転送したいときに使われるべきコードとして導入された。
		/// </summary>
		SeeOther							= 303,
		/// <summary>
		/// 未更新
		/// リクエストしたリソースは更新されていないことを示す。
		/// 例として、Id-Modified-Since: ヘッダに示されるプロキシを仕様してリクエストを行わればならないことを示す。
		/// </summary>
		NotModified							= 304,
		/// <summary>
		/// プロキシを仕様せよ
		/// レスポンスのLocation: ヘッダに示されるプロキシを使用してリクエストを行わなければならないことを示す。
		/// </summary>
		UseProxy							= 305,
		SwitchProxy							= 306,
		/// <summary>
		/// 一時的リダイレクト
		/// リクエストしたリソースは一時的に移動されているときに返される。Location: ヘッダに移動先のURLが示されている。
		/// 302(Found/旧Moved Temporarily)の規格外な使用法が横行したため、302の本来の使用法を改めて定義したもの。
		/// </summary>
		TemporaryRedirect					= 307,
		/// <summary>
		/// 恒久的リダイレクト
		/// </summary>
		PermanentRedirect					= 308,


		/// <summary>
		/// リクエストが不正である
		/// 定義されていないメソッドを使うなどクライアントのリクエストがおかしい場合に返される。
		/// </summary>
		BadRequest							= 400,
		/// <summary>
		/// 認証が必要である
		/// Basic認証やDigest認証などを行うときに使用される。
		/// 大抵のブラウザはこのステータスを受け取ると認証ダイアログを表示する。
		/// </summary>
		Unauthorized						= 401,
		/// <summary>
		/// 支払いが必要である
		/// 現在は実装されておらず、従来のために予約されているとされる。
		/// </summary>
		PaymentRequired						= 402,
		/// <summary>
		/// 禁止されている
		/// リソースにアクセスすることを拒否された。リクエストはしたがって処理できないという意味。
		/// アクセス権がない場合や、ホストがアクセス禁止処分を受けた場合などに返される。
		/// 例: 社内(イントラネット)からのみアクセスできるページに社外からアクセスしようとした。
		/// </summary>
		Forbidden							= 403,
		/// <summary>
		/// 未検出
		/// リソースが見つからなかった。
		/// 単にアクセス権が内場合などにも使用される。
		/// </summary>
		NotFound							= 404,
		/// <summary>
		/// 許可されていないメソッド
		/// 許可されていないメソッドに使用しようとした。
		/// 例: POSTメソッドの使用が許可されていない場所でPOSTメソッドを使用した場合に返される。
		/// </summary>
		MethodNotAllowed					= 405,
		/// <summary>
		/// 受理出来ない
		/// Accept関連のヘッダに受理できない内容が含まれている場合に返される。
		/// 例: サーバーは英語か日本語しか受け付けられないが、リクエストのAccept-Language: ヘッダにzh(中国語)しか含まれて居なかった。
		/// 例： サーバーはapplication/pdfを送信下が、リクエストノAccept: ヘッダにapplication/pdfが含まれて居なかった。
		/// 例： サーバーはUTF-8の文章を送信したかったが、リクエストのAccept-Charset:ヘッダにはUTF-8が含まれていなかった。
		/// </summary>
		NotAcceptable						= 406,
		/// <summary>
		/// プロキシ認証が必要である。
		/// プロキシの認証が必要な場合に返される。
		/// </summary>
		ProxyAuthenticationRequired			= 407,
		/// <summary>
		/// リクエストタイムアウト
		/// リクエストが時間居ないに完了していない場合に返される。
		/// </summary>
		RequestTimeout						= 408,
		/// <summary>
		/// 矛盾
		/// 要求は現在のリソースと矛盾するので完了できない。
		/// </summary>
		Conflict							= 409,
		/// <summary>
		/// 消滅した
		/// ファイルは恒久的に移動した。どこに行ったかもわからない。
		/// 404 Not Found と似ているが、こちらは二度と復活しない場合に示される。ただ、このコードは特殊な操作をしないと表示できないため、ファイルが消滅しても404コードを出すサイトが多い。
		/// </summary>
		Gone								= 410,
		/// <summary>
		/// 長さが必須
		/// Content-Lengthヘッダが無いのでサーバーがアクセスを拒否した場合に返される。
		/// </summary>
		LengthRequired						= 411,
		/// <summary>
		/// 前提条件で失敗した
		/// 前提条件が偽だった場合に返される。
		/// 例: リクエストのIf-UnmodifiedSince: ヘッダに書いた時刻より後に更新があった場合に返される。
		/// </summary>
		PreconditionFailed					= 412,
		/// <summary>
		/// リクエストエンティティが大きすぎる
		/// リクエストエンティティがサーバーの許容範囲を超えている場合に返す。
		/// 例: アップローダーの上限を超えたデータを送信しようとした。
		/// </summary>
		RequestEntityTooLarge				= 413,
		/// <summary>
		/// リクエストURIが大きすぎる。
		/// URIが長すぎるのでサーバーが処理を拒否した場合に返す。
		/// 例: 画像のような大きなデータをGETメソッドで送ろうとし、URIが何十kbにもなった場合に返す。(上限はサーバーに依存する。)
		/// </summary>
		RequestUriTooLong					= 414,
		/// <summary>
		/// サポートされていないメディアタイプ
		/// 指定されたメディア・タイプがサーバーでサポートされていない場合に返す。
		/// </summary>
		UnsupportedMediaType				= 415,
		/// <summary>
		/// リクエストしたレンジは範囲外にある
		/// 実ファイルのサイズを超えるデータを要求した。
		/// 例えば、リソースサイズが1024Byteしか無いのに1025Byteを取得しようとした場合などに返す。
		/// </summary>
		RequestedRangeNotSatisfiable		= 416,
		/// <summary>
		/// Expectヘッダによる拡張が失敗
		/// その拡張はレスポンスできないまたはプロキシサーバーは、次に到着するレーバーがレスポンス出来ないと判断している。
		/// 具体例として、Expect: ヘッダに100-continue以外の変なものを入れた場合やそもそもサーバーが100-Continueを扱えない場合に返す。
		/// </summary>
		ExpectationFailed					= 417,
		/// <summary>
		/// 私はティーポット!!
		/// HTCPCP/1.0の拡張ステータスコード
		/// ティーポットにコーヒーを淹れさせようとして、拒否された場合に返すとされる。ジョークコードである。
		/// </summary>
		I_AM_TEAPOT							= 418,
		AuthenticationTimeout				= 419,
		MethodFailure						= 420,
		EnhanceYourCalm						= 420,
		/// <summary>
		/// 処理できないエンティティ
		/// WebDavの拡張ステータスコード
		/// </summary>
		UnprocessableEntity					= 422,
		/// <summary>
		/// ロックされている
		/// WebDavの拡張ステータスコード。リクエストしたリソースがロックされている場合に返す。
		/// </summary>
		Locked								= 423,
		/// <summary>
		/// 依存関係で失敗
		/// WebDavの拡張ステータスコード
		/// </summary>
		FailedDependency					= 424,
		/// <summary>
		/// アップグレード要求
		/// Upgrading to Tls Within HTTP/1.1の拡張ステータスコード
		/// </summary>
		UpgradeRequired						= 426,
		TooManyRequests						= 429,
		RequestHeaderFieldsTooLarge			= 431,
		LoginTimeout						= 440,
		NoResponse							= 444,
		BlockedByWindowsParentalControls	= 450,
		Redirect							= 451,
		RequestHeaderTooLarge				= 494,
		CertError							= 495,
		NoCert								= 496,
		HttpToHttps							= 497,
		TokenExpired_Invalid				= 498,
		ClientClosedRequest					= 499,
		TokenRequired						= 499,


		/// <summary>
		/// サーバー内部エラー
		/// サーバー内部にエラーが発生した場合に返される。
		/// 例として、CGIとして動作させているプログラムに文法エラーがあったり、設定に誤りがあった場合などに返される。
		/// </summary>
		InternalServerError					= 500,
		/// <summary>
		/// 実装されていない
		/// 実装されていないメソッドを使用した。
		/// 例として、WebDavが実装されていないサーバーにたいしWebDavで使用するメソッド(MOVEやCOPY)を使用した場合に返される。
		/// </summary>
		NotImplemented						= 501,
		/// <summary>
		/// 不正なゲートウェイ
		/// ゲートウェイ・プロキシサーバーは不正な要求を受け取り、これを拒否した。
		/// </summary>
		BadGateway							= 502,
		/// <summary>
		/// サービス利用不可
		/// サービスが一時的に過負荷やメンテナンスで使用不可能である。例としてアクセスが殺到して処理不能に陥った場合に返される。
		/// </summary>
		ServiceUnavailable					= 503,
		/// <summary>
		/// ゲートウェイタイムアウト
		/// ゲートウェイ・プロキシサーバーはURIから推測されるサーバーからの適切なレスポンスが無く、タイムアウトした。
		/// </summary>
		GatewayTimeout						= 504,
		/// <summary>
		/// サポートされていないHTTPバージョン
		/// リクエストがサポートされていないHTTPバージョンである場合に返される。
		/// </summary>
		HttpVersionNotSupported				= 505,
		VariantAlsoNegotiates				= 506,
		/// <summary>
		/// 容量不足
		/// WebDavの拡張ステータスコード。リクエストを処理するために必要なストレージの容量が足りない場合に返される。
		/// </summary>
		InsufficientStorage					= 507,
		LoopDetected						= 508,
		/// <summary>
		/// 帯域幅制限超過
		/// そのサーバーに設定されている帯域幅（転送量）を使い切った場合に返される。
		/// </summary>
		BandwidethLimitExceeded				= 509,
		/// <summary>
		/// 拡張できない
		/// An HTTP Extension Frameworkで定義されている拡張ステータスコード
		/// </summary>
		NotExtended							= 510,
		NetworkAuthenticationRequired		= 511,
		OriginError							= 520,
		WebServerIsDown						= 521,
		ConnectionTimedOut					= 522,
		ProxyDeclinedRequest				= 523,
		ATimeoutOccurred					= 524,
		NetworkReadTimeoutError				= 598,
		NetworkConnectTimeoutError			= 599
	}
}
