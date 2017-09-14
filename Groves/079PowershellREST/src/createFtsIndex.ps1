Invoke-WebRequest -Method PUT -Headers @{"Authorization" = "Basic "+[System.Convert]::ToBase64String([System.Text.Encoding]::UTF8.GetBytes("Administrator:password")); "Content-Type"="application/json"} -Uri http://Administrator:password@localhost:8094/api/index/medical-condition -Body '{
  "type": "fulltext-index",
  "name": "medical-condition",
  "sourceType": "couchbase",
  "sourceName": "default",
  "planParams": {
    "maxPartitionsPerPIndex": 171
  },
  "params": {
    "doc_config": {
      "mode": "type_field",
      "type_field": "type"
    },
    "mapping": {
      "default_analyzer": "standard",
      "default_datetime_parser": "dateTimeOptional",
      "default_field": "_all",
      "default_mapping": {
        "dynamic": true,
        "enabled": false
      },
      "default_type": "_default",
      "index_dynamic": true,
      "store_dynamic": false,
      "types": {
        "patient": {
          "dynamic": false,
          "enabled": true,
          "properties": {
            "information": {
              "dynamic": false,
              "enabled": true,
              "properties": {
                "firstname": {
                  "dynamic": false,
                  "enabled": true,
                  "fields": [
                    {
                      "analyzer": "",
                      "include_in_all": true,
                      "include_term_vectors": true,
                      "index": true,
                      "name": "firstname",
                      "store": true,
                      "type": "text"
                    }
                  ]
                },
                "lastname": {
                  "dynamic": false,
                  "enabled": true,
                  "fields": [
                    {
                      "analyzer": "",
                      "include_in_all": true,
                      "include_term_vectors": true,
                      "index": true,
                      "name": "lastname",
                      "store": true,
                      "type": "text"
                    }
                  ]
                }
              }
            },
            "notes": {
              "dynamic": false,
              "enabled": true,
              "properties": {
                "message": {
                  "dynamic": false,
                  "enabled": true,
                  "fields": [
                    {
                      "analyzer": "",
                      "include_in_all": true,
                      "include_term_vectors": true,
                      "index": true,
                      "name": "message",
                      "store": true,
                      "type": "text"
                    }
                  ]
                }
              }
            }
          }
        }
      }
    },
    "store": {
      "kvStoreName": "mossStore"
    }
  },
  "sourceParams": {}
}'