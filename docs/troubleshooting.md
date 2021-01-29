# Troubleshooting

This will fill up gradually with Troubleshooting tips as people run into common
ones.

# VoiceAttack does not understand me / mishears me / fires random commands

There is [a thread on the VoiceAttack
forums](https://forum.voiceattack.com/smf/index.php?topic=2667.msg12197#msg12197)
on how to set up your microphone and the speech recognition engine to work best.

If your microphone is bad and you still get erroneous recognitions when you are
not speaking it is probably going to recognize the same command every time. You
can remedy that by blocking the voice trigger.

1. Create a new command in your custom profile.
1. Set the “when I say” field to the trigger that gets misrecognized.

Adding the “Other” → “VoiceAttack Action” → “Ignore an Unrecognized Word or
Phrase” action will also hide it from the VoiceAttack log when it is (wrongly)
recognized. You might or might not want that.

Example for the “cruise” voice trigger of the Supercruise command:

![[troubleshooting-remove-trigger.png]]

Alternatively you can raise the minimum confidence level and call the underlying
command to make misfires less likely:

![[troubleshooting-raise-min-confidence.png]]

There are a few examples in the [Custom Profile
Example](../installing#use-the-profile-example).