# DB Schema TSV Rules (もぐもぐマネジメント)

`Assets/db/SchemaTSV/` のTSVは、**列定義と実行SQLを同時に持つ**運用です。

## 目的
- 人間が「列定義」を読みやすい（仕様として残る）
- 実装は `sql` 列に入ったSQLを実行するだけ（コードは短く保つ）

## ルール
1. TSVは **テーブルごとに1ファイル**
2. 1行目はヘッダー固定：
   `table, ordinal, column, type, notnull, pk, autoinc, default, sql`
3. `sql` 列には **1ファイルにつき1文**のみ入れる  
   - 通常：先頭データ行（ordinal=1）だけに入れる
   - seed等で列定義が不要な場合：1行だけでもOK（column等は空でOK）
4. `sql` 文末に `;` は付けない
5. コメント（-- / /* */）はSQL内に書かない

## 補足
- `sql` を実行するだけなので、TSV上の列定義は「仕様・レビュー用」です。
- 将来マイグレーションが必要なら、`schema_version` とバージョン別TSV/SQLを追加して拡張します。
